using System;
using System.Collections.Generic;
using System.Linq;

namespace Federation.Core
{
    public enum AttachOrDetach : byte { Attach = 1, Detach = 0 }

    public class ContentServiceImpl : IContentService
    {
        public void UpdateContentCache(Content content)
        {
            if (content.GroupId.HasValue)
                CachService.DropViewModelByModel(content.GroupId.Value);
            if (content.AuthorId.HasValue)
                CachService.DropViewModelByModel(content.AuthorId.Value);
        }

        public Content Attach(Guid contentId, Guid? currUserId, AttachDetachTarget target)
        {
            return AttachDetach(contentId, currUserId, target, AttachOrDetach.Attach);
        }

        public Content Detach(Guid contentId, Guid? currUserId, AttachDetachTarget target)
        {
            return AttachDetach(contentId, currUserId, target, AttachOrDetach.Detach);
        }

        private Content AttachDetach(Guid contentId, Guid? currUserId, AttachDetachTarget target, AttachOrDetach wtf) // TODO: при ретрансляции постов принимать ИД групп/юзеров, где пост прикрепить/открепить
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == contentId);
            if (content == null)
                throw new BusinessLogicException("Нет контента с указанным идентификатором");

            var attach = content.Attach;

            if (currUserId.HasValue)
            {
                if (content.GroupId.HasValue)
                {
                    var gm = GroupService.UserInGroup(currUserId.Value, content.Group.Id);

                    if (gm == null)
                        throw new BusinessLogicException("Вы не имеете права");
                    if (gm.State != (byte)GroupMemberState.Moderator)
                        throw new BusinessLogicException("Вы не имеете права");                    
                }
                else
                {
                    if (content.AuthorId.HasValue)
                    {
                        if (content.AuthorId.Value != currUserId.Value)
                            throw new BusinessLogicException("Вы не имеете права");
                    }
                    else
                        throw new BusinessLogicException("Вы не имеете права");
                }
            }

            if (attach == null) 
            {
                switch (wtf)
                {
                    case AttachOrDetach.Attach:
                        attach = new Attach { Content = content };
                        DataService.PerThread.AttachSet.AddObject(attach);
                        break;

                    case AttachOrDetach.Detach:
                        return content;             
                }
            }

            switch (target)
            {
                case AttachDetachTarget.Group:
                    switch (wtf)
                    {
                        case AttachOrDetach.Attach:
                            attach.Groups.Add(content.Group);
                            break;

                        case AttachOrDetach.Detach:
                            attach.Groups.Remove(content.Group);
                            break;
                    }
                    break;

                case AttachDetachTarget.User:
                    switch (wtf)
                    {
                        case AttachOrDetach.Attach:
                            attach.Users.Add(content.Author);
                            break;

                        case AttachOrDetach.Detach:
                            attach.Users.Remove(content.Author);
                            break;
                    }
                    break;
            }

            DataService.PerThread.SaveChanges();
            UpdateContentCache(content);

            return content;
        }

        public Post CreatePost(Guid? groupId, Guid? authorId, string title, string text, string tagsString, bool isDraft, bool isExternal)
        {
            var post = new Post
            {
                AuthorId = authorId,
                GroupId = groupId,
                IsExternal = isExternal,
                CreationDate = DateTime.Now,
                PublishDate = !isDraft ? DateTime.Now : (DateTime?)null,
                Tags = new List<Tag>(),
                Title = title,
                Text = text
            };

            foreach (var tag in TagsHelper.ConvertStringToTagList(tagsString, post.GroupId))
                post.Tags.Add(tag);

            if (!authorId.HasValue)
            {
                if (!groupId.HasValue)
                    throw new BusinessLogicException("Укажите принадлежность поста к группе или пользователю");

                post.State = isDraft ? (byte)ContentState.Draft : (byte)ContentState.Approved;
            }
            else
            {
                if (groupId.HasValue)
                {
                    var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == groupId.Value);
                    if (group == null)
                        throw new BusinessLogicException("Указан неверный идентификатор группы");

                    var gm = GroupService.UserInGroup(authorId.Value, group, true);

                    if (isDraft)
                        post.State = (byte)ContentState.Draft;
                    else
                    {
                        if (group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration))
                            post.State = gm.State == (byte)GroupMemberState.Moderator
                                             ? (byte)ContentState.Approved
                                             : (byte)ContentState.Premoderated;
                        else
                        {
                            if (isExternal)
                                post.State = gm.State == (byte)GroupMemberState.Moderator
                                                 ? (byte)ContentState.Approved
                                                 : (byte)ContentState.Premoderated;
                            else
                                post.State = (byte)ContentState.Approved;
                        }
                    }
                }
                else                
                    post.State = isDraft ? (byte)ContentState.Draft : (byte)ContentState.Approved;
                
            }

            DataService.PerThread.ContentSet.AddObject(post);
            DataService.PerThread.SaveChanges();
            UpdateContentCache(post);

            return post;
        }

        public Post UpdatePost(Guid postId, Guid? editorId, string newTitle, string newText, string newTagsString, bool isDraft)
        {
            var post = DataService.PerThread.ContentSet.OfType<Post>().SingleOrDefault(p => p.Id == postId);
            if (post == null)
                throw new BusinessLogicException("Не найден пост с указанным идентификатором");

            if (!editorId.HasValue)
                post.State = isDraft ? (byte)ContentState.Draft : (byte)ContentState.Approved;
            else
            {
                if (post.GroupId.HasValue)
                {
                    var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == post.GroupId.Value);
                    if (group == null)
                        throw new BusinessLogicException("Указан неверный идентификатор группы");

                    var gm = GroupService.UserInGroup(editorId.Value, group, true);

                    if (isDraft)
                        post.State = (byte)ContentState.Draft;
                    else
                    {
                        if (group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration))
                        {
                            if (gm.State == (byte)GroupMemberState.Moderator)
                                post.State = (byte)ContentState.Approved;
                            else
                            {
                                if (!post.AuthorId.HasValue)
                                    throw new BusinessLogicException("Вы не являетесь автором поста");
                                if (post.AuthorId.Value != editorId.Value)
                                    throw new BusinessLogicException("Вы не являетесь автором поста");

                                post.State = (byte)ContentState.Premoderated;
                            }
                        }
                        else
                        {
                            if (post.IsExternal)
                                post.State = gm.State == (byte)GroupMemberState.Moderator
                                                 ? (byte)ContentState.Approved
                                                 : (byte)ContentState.Premoderated;
                            else
                                post.State = (byte)ContentState.Approved;
                        }
                    }
                }
                else
                {
                    if (!post.AuthorId.HasValue)
                        throw new BusinessLogicException("Вы не являетесь автором поста");
                    if (post.AuthorId.Value != editorId.Value)
                        throw new BusinessLogicException("Вы не являетесь автором поста");

                    post.State = isDraft ? (byte)ContentState.Draft : (byte)ContentState.Approved;
                }
            }

            if (!post.PublishDate.HasValue)
                post.PublishDate = DateTime.Now;

            post.Title = newTitle;
            post.Text = newText;
            post.Tags.Clear();

            var tags = TagsHelper.ConvertStringToTagList(newTagsString, post.GroupId);
            foreach (var tag in tags)
                if (!post.Tags.Contains(tag))
                    post.Tags.Add(tag);

            DataService.PerThread.SaveChanges();
            UpdateContentCache(post);

            return post;
        }

        public void ToggleDiscussion(bool show, Guid contentId, Guid userId)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == contentId);

            ToggleDiscussion(show, content, userId);

            DataService.PerThread.SaveChanges();
        }

        public void ToggleDiscussion(bool show, Content content, Guid userId)
        {
            if (content == null)
                throw new BusinessLogicException("Не найден указанный контент");

            var group = content.Group;

            if (IsAuthor(content, userId))
            {
                if (group == null)
                    content.IsDiscussionClosed = !show;
                else
                {
                    GroupService.UserInGroup(userId, group, true);

                    content.IsDiscussionClosed = !show;
                }
            }
            else
            {
                if (group == null)
                    throw new BusinessLogicException("Недостаточно прав на изменение состояния обсуждения");
                
                var member = GroupService.UserInGroup(userId, group);
                if (member.State != (byte)GroupMemberState.Moderator)
                    throw new BusinessLogicException("Недостаточно прав на изменение состояния обсуждения");

                content.IsDiscussionClosed = !show;
            }
        }

        public void Delete(Guid contentId, Guid userId)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == contentId);

            Delete(content, userId);

            DataService.PerThread.SaveChanges();
        }

        public void Delete(Content content, Guid userId)
        {
            if (content == null)
                throw new BusinessLogicException("Не найден указанный контент");

            if (content.GroupId.HasValue)
                GroupService.UserInGroup(userId, content.Group, true);

            if (!IsAuthor(content, userId))
                throw new BusinessLogicException("Удалять контент могут только авторы");

            content.State = (byte)ContentState.Deleted;
            UpdateContentCache(content);
        }

        public void Restore(Guid contentId, Guid userId)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == contentId);

            Restore(content, userId);

            DataService.PerThread.SaveChanges();
        }

        public void Restore(Content content, Guid userId)
        {
            if (content == null)
                throw new BusinessLogicException("Не найден указанный контент");

            if (!IsAuthor(content, userId))
                throw new BusinessLogicException("Восстанавливать контент могут только авторы");

            // Описываем исключения, когда нельзя восстановить
            if (content.State != (byte)ContentState.Deleted)
                throw new BusinessLogicException("Указанный контент не является удаленным");

            if (content is Election)
                throw new BusinessLogicException("Нельзя восстановить удаленные выборы");
            if (content is Poll)
                throw new BusinessLogicException("Нельзя восстановить удаленное голосование");
            if (content is Petition)
                throw new BusinessLogicException("Нельзя восстановить удаленную петицию");

            if (!content.GroupId.HasValue)
                content.State = (byte)ContentState.Approved;
            else
            {
                var gm = GroupService.UserInGroup(userId, content.Group, true);

                if (!content.Group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration))
                {
                    content.State = (byte)ContentState.Approved;

                    if (content is Post)
                        if ((content as Post).IsExternal)
                            content.State = gm.State == (byte)GroupMemberState.Moderator
                                             ? (byte)ContentState.Approved
                                             : (byte)ContentState.Premoderated;
                }
                else
                    content.State = gm.State == (byte)GroupMemberState.Moderator
                                        ? (byte)ContentState.Approved
                                        : (byte)ContentState.Premoderated;
            }

            UpdateContentCache(content);
        }

        public void ToggleBlock(bool block, Guid contentId, Guid userId)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == contentId);

            ToggleBlock(block, content, userId);

            DataService.PerThread.SaveChanges();
        }

        public void ToggleBlock(bool block, Content content, Guid userId)
        {
            if (content == null)
                throw new BusinessLogicException("Не найден указанный контент");

            var group = content.Group;
            if (group == null)
                throw new BusinessLogicException("Можно блокировать только контент, привязанный к группе");

            var member = GroupService.UserInGroup(userId, group, true);
            if (member.State != (byte)GroupMemberState.Moderator)
                throw new BusinessLogicException("Только модераторы имеют право блокировать контент");

            if (block)
                content.State = (byte)ContentState.Blocked;
            else if (content.State == (byte)ContentState.Blocked)
                content.State = (byte)ContentState.Approved;

            UpdateContentCache(content);
        }

        public Content Publish(Guid contentId, Guid userId)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == contentId);

            Publish(content, userId);

            DataService.PerThread.SaveChanges();

            return content;
        }

        public void Publish(Content content, Guid userId)
        {
            if (content == null)
                throw new BusinessLogicException("Не найден указанный контент");

            if (content.State != (byte)ContentState.Draft && content.State != (byte)ContentState.Premoderated)
                throw new BusinessLogicException("Данный контент нельзя публиковать");

            if (content.GroupId.HasValue) // Контент привязан к группе
            {
                var member = GroupService.UserInGroup(userId, content.GroupId.Value, true);

                if (content.Group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration))
                {
                    if (member.State == (byte)GroupMemberState.Moderator)
                        content.State = (byte)ContentState.Approved;
                    else
                    {
                        if (!IsAuthor(content, userId))
                            throw new BusinessLogicException("Нельзя публиковать чужой контент");

                        content.State = (byte)ContentState.Premoderated;
                    }
                }
                else
                {                    
                    if (content is Post && (content as Post).IsExternal)
                        content.State = 
                            member.State == (byte)GroupMemberState.Moderator ? (byte)ContentState.Approved : (byte)ContentState.Premoderated;
                    else
                        content.State = (byte)ContentState.Approved;
                }
            }
            else
            {
                if (!IsAuthor(content, userId))
                    throw new BusinessLogicException("Нельзя публиковать чужой контент");

                content.State = (byte)ContentState.Approved;
            }

            content.PublishDate = DateTime.Now;

            if (content.State == (byte)ContentState.Approved)
            {
                if (content is Poll)
                    VotingService.StartPoll(content.Id, userId);
                else if (content is Survey)
                    VotingService.StartSurvey(content.Id);
            }

            UpdateContentCache(content);
        }

        public void Unpublish(Guid contentId, Guid userId)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == contentId);

            Unpublish(content, userId);

            DataService.PerThread.SaveChanges();
        }

        public void Unpublish(Content content, Guid userId)
        {
            if (content == null)
                throw new BusinessLogicException("Не найден указанный контент");

            if (content.State != (byte)ContentState.Approved && content.State != (byte)ContentState.Premoderated)
                throw new BusinessLogicException("Нельзя отменить публикацию данного контента");

            if (content is Poll)
                throw new BusinessLogicException("Нельзя отменять публикацию голосований");

            if (!IsAuthor(content, userId))
                throw new BusinessLogicException("Нельзя отменять публикацию чужого контента");

            if (content.GroupId.HasValue) // Контент привязан к группе
                GroupService.UserInGroup(userId, content.GroupId.Value, true);

            content.State = (byte)ContentState.Draft;
            UpdateContentCache(content);
        }

        public bool IsAuthor(Guid contentId, Guid userId)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(x => x.Id == contentId);

            return IsAuthor(content, userId);
        }

        public bool IsAuthor(Content content, Guid userId)
        {
            if (content == null)
                throw new BusinessLogicException("Невозможно проверить является ли указанный пользователь автором, т.к. не найден указанный контент");

            return content.AuthorId == userId;
        }

        public void ClearOldDeletedContent()
        {
        }
    }
}