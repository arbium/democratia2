using System;
using System.Collections.Generic;
using System.Linq;

namespace Federation.Core
{
    public static class TagService
    {
        public static void UpdateTagsWeight()
        {
            DataService.PerThread.TagSet.Where(t => t.GroupId != null).ToList()
                .ForEach(tag => { tag.Weight = tag.Contents.Count(x => x.State == (byte)ContentState.Approved); });
            DataService.PerThread.SaveChanges();
        }

        public static Tag AddTagByUser(string title, string description, GroupMember groupMember)
        {
            var tag = GetTag(title, groupMember.Group, true);

            if (groupMember.State == (byte)GroupMemberState.Approved)
            {
                if (tag.TopicState == (byte)TopicState.NotTopic)
                {
                    tag.TopicState = (byte)TopicState.ProposedTopic;
                    tag.Description = description;
                }
                else if (tag.TopicState == (byte)TopicState.GroupTopic)
                    throw new ValidationException("Такая тема группы уже существует");
                else if (tag.TopicState == (byte)TopicState.ProposedTopic)
                    throw new ValidationException("Такая тема группы уже преждложена и ожидает решения модераторов.");
            }
            else if (groupMember.State == (byte)GroupMemberState.Moderator)
            {
                if (tag.TopicState == (byte)TopicState.NotTopic || tag.TopicState == (byte)TopicState.ProposedTopic)
                {
                    tag.TopicState = (byte)TopicState.GroupTopic;
                    tag.Description = description;
                    tag.IsRecommended = true;
                }
                else if (tag.TopicState == (byte)TopicState.GroupTopic)
                    throw new ValidationException("Такая тема группы уже существует");
            }

            DataService.PerThread.SaveChanges();

            if (groupMember.Group.State == (byte)GroupMemberState.Approved)
            {
                var date = DateTime.Now;
                MessageService.SendToGroup(groupMember.Group,
                new MessageStruct
                {
                    Text = MessageComposer.ComposeGroupModeratorNotice(groupMember.Group.Id, "Поступило предложение на добавление новой темы группы: <a href='" + tag.GetTopicUrl(false) + "'>" + tag.Title + "</a>"),
                    Type = (byte)MessageType.GroupModeratorNotice,
                    Date = date
                }
                , GroupMessageRecipientType.Moderators);
            }

            return tag;
        }

        public static bool UpdateTagByUser(Guid tagid, string title, string description, GroupMember groupMember)
        {
            var tag = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == tagid);

            if (tag == null)
                throw new BusinessLogicException("Указанная тема не найдена");

            if (groupMember.State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("У вас недастаточно прав для редактирования темы");

            var result = UpdateTag(tag, title, description);

            DataService.PerThread.SaveChanges();

            return result;
        }

        public static Tag GetTag(string tagTitle, Group group, bool createIfNotExist = false)
        {
            tagTitle = new string(tagTitle.ToCharArray().Where(c => !char.IsPunctuation(c)).ToArray()).Trim();
            var tag = group.Tags.SingleOrDefault(x => x.LowerTitle == tagTitle.ToLower());

            if (tag == null && createIfNotExist)
            {
                tag = CreateTag(tagTitle, "", group.Id);
            }
            DataService.PerThread.SaveChanges();

            return tag;
        }

        public static Tag GetTag(string tagTitle, Guid? groupId, bool createIfNotExist = false)
        {
            Tag tag = null;
            tagTitle = new string(tagTitle.ToCharArray().Where(c => !char.IsPunctuation(c)).ToArray()).Trim();

            if (groupId.HasValue)
            {
                //Очень странная бага в Entity при группе = null не выбираются значения с groupId = null
                tag = DataService.PerThread.TagSet.SingleOrDefault(t => t.LowerTitle == tagTitle.ToLower() && t.GroupId == groupId);
            }
            else
            {
                tag = DataService.PerThread.TagSet.SingleOrDefault(t => t.LowerTitle == tagTitle.ToLower() && t.GroupId == null);
            }

            if (tag == null && createIfNotExist)
            {
                tag = CreateTag(tagTitle, "", groupId);
            }
            DataService.PerThread.SaveChanges();

            return tag;
        }

        public static IList<Tag> GetTags(Guid? groupId, params Guid[] topicIds)
        {
            var tags = DataService.PerThread.TagSet.Where(x => topicIds.Contains(x.Id)).ToList();

            if (groupId != null)
            {
                IList<Tag> result = new List<Tag>();
                foreach (var tag in tags)
                {
                    if (tag.GroupId != groupId)
                        throw new BusinessLogicException(String.Format("Тег {0} не принадлежит группе #{1}", tag.Title, groupId));

                    result.Add(tag);
                }
                return result;
            }

            return tags;
        }

        public static void UserSetTagRecomendation(Guid tagId, Guid userId, Guid groupId, bool recomended)
        {
            var gm = GroupService.UserInGroup(userId, groupId, true);

            if (gm.State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Только модераторы имеют право устанваливать рекомендуемость тем");

            SetTagRecomendation(tagId, recomended);
        }

        public static void SetTagRecomendation(Guid tagId, bool recomended)
        {
            var tag = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == tagId);
            if (tag == null)
                throw new BusinessLogicException("Тема с указанным идентификатором не найдена");
            SetTagRecomendation(tag, recomended);
            DataService.PerThread.SaveChanges();
        }

        public static void SetTagRecomendation(Tag tag, bool recomended)
        {
            if (tag.TopicState == (byte)TopicState.GroupTopic)
                throw new BusinessLogicException("Нельзя сменить настройки рекомендации у темы группы");

            tag.IsRecommended = recomended;
        }

        public static Tag CreateTag(string tagTitle, string description, Guid? groupId)
        {
            Guid tmp;
            if (Guid.TryParse(tagTitle, out tmp))
                throw new BusinessLogicException("Некорректное название тэга");

            var tag = new Tag(tagTitle)
                {
                    Description = description,
                    GroupId = groupId,
                };

            DataService.PerThread.TagSet.AddObject(tag);

            return tag;
        }

        public static bool UpdateTag(Tag tag, string title, string description)
        {
            var suchtag = GetTag(title, tag.Group);

            if (suchtag == null)
            {
                tag.Description = description;
                tag.ChangeTitle(title);
                return true;
            }

            return false;
        }

        public static List<string> FindMatchingTags(string query, string groupId, bool showTopics)
        {
            List<string> matchingTags;
            if (string.IsNullOrWhiteSpace(groupId))
            {
                matchingTags = DataService.PerThread.TagSet.Where(t => t.LowerTitle.Contains(query) && t.GroupId == null).Select(x => x.Title).ToList();
            }
            else
            {
                Guid id;
                if (!Guid.TryParse(groupId, out id))
                    throw new BusinessLogicException("Неверный идентификатор группы!");

                if (DataService.PerThread.GroupSet.SingleOrDefault(gr => gr.Id == id) == null)
                    throw new BusinessLogicException("Неверный идентификатор группы!");
                
                if (showTopics)
                    matchingTags = DataService.PerThread.TagSet.Where(t => t.LowerTitle.Contains(query) && t.GroupId == id).OrderByDescending(t => t.TopicState).ThenBy(t => t.IsRecommended).ThenByDescending(t => t.Weight).Select(x => x.Title).ToList();
                else
                    matchingTags = DataService.PerThread.TagSet.Where(t => t.LowerTitle.Contains(query) && t.GroupId == id && t.TopicState != (byte)TopicState.GroupTopic).OrderBy(t => t.IsRecommended).ThenByDescending(t => t.Weight).Select(x => x.Title).ToList();
            }

            return matchingTags;
        }
    }
}
