using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserSelfDelegationViewModel
    {        
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public string GroupLogo { get; set; }
        public int GroupMemberCount { get; set; }

        public readonly IList<TagViewModel> Tags = new List<TagViewModel>();
        public readonly IList<TagViewModel> ExpertTags = new List<TagViewModel>();
        public readonly IList<TagViewModel> DelegatedTags = new List<TagViewModel>();
        public readonly Dictionary<Guid, int> VotersCountForTag = new Dictionary<Guid, int>();

        public UserSelfExpertDelegate_ExpertViewModel Expert { get; private set; }        

        public UserSelfDelegationViewModel()
        {
            Expert = new UserSelfExpertDelegate_ExpertViewModel();
        }

        public UserSelfDelegationViewModel(Guid id, string groupUrl)
        {
            var group = GroupService.GetGroupByLabelOrId(groupUrl);
            if (group == null)
                throw new BusinessLogicException("Не найдена данная группа");

            var expertGroupMember = DataService.PerThread.GroupMemberSet.SingleOrDefault(x => x.UserId == id && x.GroupId == group.Id);
            if (expertGroupMember == null)
                throw new BusinessLogicException("Данный человек не состоит в данной группе");

            var expert = DataService.PerThread.ExpertSet.SingleOrDefault(x => x.GroupMember.Id == expertGroupMember.Id);
            if (expert == null)
                expert = DelegationService.CreateExpertWithoutSaving(expertGroupMember);

            Expert = new UserSelfExpertDelegate_ExpertViewModel(expert);

            GroupUrl = groupUrl;
            GroupName = expert.GroupMember.Group.Name;
            GroupLogo = ImageService.GetImageUrl<Group>(expert.GroupMember.Group.Logo);

            var groupId = expert.GroupMember.GroupId;
            GroupMemberCount = DataService.PerThread.GroupMemberSet.Count(x => x.GroupId == groupId);

            var tags = DataService.PerThread.TagSet.Where(x => x.GroupId == groupId && x.TopicState == (byte)TopicState.GroupTopic);

            foreach (var tag in tags)
            {
                if(expert.Tags.Contains(tag))
                    ExpertTags.Add(new TagViewModel(tag));
                else if(DataService.PerThread.ExpertVoteSet.SingleOrDefault(x => x.GroupMemberId == expertGroupMember.Id && x.TagId == tag.Id) != null)
                    DelegatedTags.Add(new TagViewModel(tag));
                else
                    Tags.Add(new TagViewModel(tag));

                var votersCount = DataService.PerThread.ExpertVoteSet.Count(x => x.TagId == tag.Id && x.ExpertId == Expert.Id);
                VotersCountForTag.Add(tag.Id, votersCount);
            }
        }
    }

    public class UserSelfExpertDelegate_ExpertViewModel
    {
        public string Avatar { get; set; }
        public Guid Id { get; set; }
        public string Info { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }

        public UserSelfExpertDelegate_ExpertViewModel()
        {
        }

        public UserSelfExpertDelegate_ExpertViewModel(Expert expert)
        {
            if (expert != null)
            {
                Avatar = ImageService.GetImageUrl<User>(expert.GroupMember.User.Avatar);
                Id = expert.Id;
                Info = expert.Info;
                Name = expert.GroupMember.User.FullName;
                UserId = expert.GroupMember.UserId;
            }
        }
    }
}
