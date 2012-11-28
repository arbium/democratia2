using System;
using System.Collections.Generic;
using System.Linq;

namespace Federation.Core
{
    public static class DelegationService
    {
        private static Tag GetTopic(Guid tagId)
        {
            return DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == tagId && x.GroupId.HasValue && x.TopicState == (byte)TopicState.GroupTopic);
        }

        public static void EditExpertInfo(Guid expertUserId, string groupLabelOrId, string expertInfo)
        {
            var group = GroupService.GetGroupByLabelOrId(groupLabelOrId);

            var expertGroupMember = GroupService.UserInGroup(expertUserId, group.Id, true);
            if (expertGroupMember == null)
                throw new BusinessLogicException("Вы не состоите в этой группе");

            var expert = DataService.PerThread.ExpertSet.SingleOrDefault(x => x.GroupMember.Id == expertGroupMember.Id) ??
                         CreateExpertWithoutSaving(expertGroupMember);

            expert.Info = expertInfo;
            
            DataService.PerThread.SaveChanges();
        }

        public static void BecomeExpert(Guid expertUserId, Guid tagId)
        {
            var topic = GetTopic(tagId);
            if (topic == null)
                throw new BusinessLogicException("Не найдена данная тема группы");

            var expertGroupMember = GroupService.UserInGroup(expertUserId, topic.Group, true);

            var vote = DataService.PerThread.ExpertVoteSet.SingleOrDefault(x => x.GroupMember.Id == expertGroupMember.Id && x.TagId == tagId);
            if (vote != null)
                DataService.PerThread.ExpertVoteSet.DeleteObject(vote);
                //throw new BusinessLogicException("Вы делегировали голос " + vote.Expert.GroupMember.User.FullName);

            var expert = DataService.PerThread.ExpertSet.SingleOrDefault(x => x.GroupMember.Id == expertGroupMember.Id) ??
                         CreateExpertWithoutSaving(expertGroupMember);

            if (expert.Tags.Contains(topic))
                throw new BusinessLogicException("Вы не можете стать экспертом по данному тегу, т.к вы уже эксперт по нему");

            expert.Tags.Add(topic);

            DataService.PerThread.SaveChanges();
        }

        public static void CeaseToBeExpert(Guid expertUserId, Guid tagId)
        {
            var topic = GetTopic(tagId);
            if (topic == null)
                throw new BusinessLogicException("Не найдена данная тема группы");

            var expertGroupMember = GroupService.UserInGroup(expertUserId, topic.GroupId.Value, true);
            if (expertGroupMember == null)
                throw new BusinessLogicException("Вы не состоите в этой группе");

            var expert = DataService.PerThread.ExpertSet.SingleOrDefault(x => x.GroupMember.Id == expertGroupMember.Id);
            if(expert == null)
                throw new BusinessLogicException("Вы не являетесь экспертом в данной группе");

            if (expert.Tags.Contains(topic))
                expert.Tags.Remove(topic);
            else
                throw new BusinessLogicException("Вы не являетесь экспертом по данному тегу");

             DataService.PerThread.SaveChanges();
        }

        public static Expert CreateExpertWithoutSaving(GroupMember expertGroupMember)
        {
            var expert = new Expert { GroupMember = expertGroupMember };
            DataService.PerThread.ExpertSet.AddObject(expert);

            return expert;
        }

        public static void RemoveExpert(string groupLabelOrId, Guid expertUserId)
        {
            var group = GroupService.GetGroupByLabelOrId(groupLabelOrId);
            if (group == null)
                throw new Exception("Не найдена данная группа");

            var expertGroupMember = GroupService.UserInGroup(expertUserId, group.Id);
            if (expertGroupMember == null)
                throw new BusinessLogicException("Вы не состоите в данной группе");

            var expert = DataService.PerThread.ExpertSet.SingleOrDefault(e => e.GroupMember.GroupId == group.Id && e.GroupMember.UserId == expertUserId);
            if (expert == null)
                throw new BusinessLogicException("Вы не являетесь экспертом в данной группе");

            IList<ExpertVote> deletingVotes = DataService.PerThread.ExpertVoteSet.Where(x => x.ExpertId == expert.Id).ToList();
            foreach (var deletingVote in deletingVotes)
                DataService.PerThread.ExpertVoteSet.DeleteObject(deletingVote);

            expert.ExpertVotes.Clear();
            expert.Tags.Clear();

            DataService.PerThread.ExpertSet.DeleteObject(expert);
            DataService.PerThread.SaveChanges();
        }

        public static void Delegate(Guid voterUserId, Guid expertId, Guid tagId)
        {
            var tag = GetTopic(tagId);
            if (tag == null)
                throw new BusinessLogicException("Данный тег не является топиком");

            var expert = DataService.PerThread.ExpertSet.SingleOrDefault(x => x.Id == expertId);
            if (expert == null)
                throw new BusinessLogicException("Не найден человек эксперт!");

            var groupId = expert.GroupMember.GroupId;

            var voter = DataService.PerThread.GroupMemberSet.SingleOrDefault(x => x.UserId == voterUserId && x.GroupId == groupId);
            if (voter == null)
                throw new BusinessLogicException("Не найден человек, делегирующий голос!");

            if (voter.GroupId != groupId)
                throw new BusinessLogicException("Данный тег не принадлежит этой группе");
            if (voter.Expert != null && voter.Expert.Tags.Contains(tag))
                    throw new BusinessLogicException("По этому тэгу вы сами являетесь экспертом");

            var expertVote = DataService.PerThread.ExpertVoteSet.SingleOrDefault(x => x.GroupMemberId == voter.Id && x.TagId == tagId);
            if (expertVote != null)
            {
                expertVote.Expert = expert;
                DataService.PerThread.SaveChanges();
            }
            else
            {
                expertVote = new ExpertVote
                    {
                        Date = DateTime.Now,
                        ExpertId = expertId,
                        GroupMemberId = voter.Id,
                        TagId = tagId
                    };

                DataService.PerThread.ExpertVoteSet.AddObject(expertVote);
                DataService.PerThread.SaveChanges();
            }
        }

        public static void Undelegate(Guid voterId, Guid tagId)
        {
            var tag = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == tagId);
            if (tag == null)
                throw new BusinessLogicException("Данный тег не найден");

            var member = DataService.PerThread.GroupMemberSet.SingleOrDefault(x => x.UserId == voterId && tag.GroupId == x.GroupId);
            if (member == null)
                throw new BusinessLogicException("Данный пользователь не найден");

            var deletingExpertVote = DataService.PerThread.ExpertVoteSet.SingleOrDefault(x => x.TagId == tagId && x.GroupMember.UserId == voterId);
            if (deletingExpertVote == null)
                throw new BusinessLogicException("Вы не делегировали голос по данному тегу");

            DataService.PerThread.ExpertVoteSet.DeleteObject(deletingExpertVote);
            DataService.PerThread.SaveChanges();
        }

    }
}