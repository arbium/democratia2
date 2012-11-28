using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserDelegationViewModel
    {        
        public Guid Id { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public string GroupLogo { get; set; }
        public bool UserIsApprovedMember { get; set; }
        public int GroupMemberCount { get; set; }
        
        public readonly IList<TagViewModel> CheckedTags = new List<TagViewModel>();
        public readonly IList<TagViewModel> NotCheckedTags = new List<TagViewModel>();
        public readonly IList<TagViewModel> DisabledTags = new List<TagViewModel>();
        public readonly IList<TagViewModel> AnonymouseTags = new List<TagViewModel>();
        public readonly Dictionary<Guid, int> VotersCountForTag = new Dictionary<Guid, int>();

        public UserExpertDelegate_ExpertViewModel Expert { get; private set; }

        public UserDelegationViewModel()
        {
            Expert = new UserExpertDelegate_ExpertViewModel();
        }

        public UserDelegationViewModel(Guid id, string groupUrl)
        {
            var group = GroupService.GetGroupByLabelOrId(groupUrl);
            if(group == null)
                throw new BusinessLogicException("Не найдена данная группа");
            
            var expertGroupMember = DataService.PerThread.GroupMemberSet.SingleOrDefault(x => x.UserId == id && x.GroupId == group.Id);
            if(expertGroupMember == null)
                throw new BusinessLogicException("Данный человек не состоит в данной группе");

            var expert = DataService.PerThread.ExpertSet.SingleOrDefault(x => x.GroupMember.Id == expertGroupMember.Id);
            if (expert == null)
                throw new BusinessLogicException("Данный человек не явлется экспертом в этой группе");

            //expertId = expert.Id;
            Expert = new UserExpertDelegate_ExpertViewModel(expert);

            GroupUrl = expert.GroupMember.Group.Url;
            GroupName = expert.GroupMember.Group.Name;
            GroupLogo = ImageService.GetImageUrl<Group>(expert.GroupMember.Group.Logo);
            var groupId = expert.GroupMember.GroupId;
            GroupMemberCount = DataService.PerThread.GroupMemberSet.Count(x => x.GroupId == groupId);

            GroupMember userInGroup = null;
            if(UserContext.Current != null)
            { 
                var userId = UserContext.Current.Id;
                userInGroup =
                    DataService.PerThread.GroupMemberSet.SingleOrDefault(x => x.UserId == userId && x.GroupId == groupId);
            }

            if (userInGroup != null)
                UserIsApprovedMember = (userInGroup.State == (byte)GroupMemberState.Approved || userInGroup.State == (byte)GroupMemberState.Moderator);

            foreach (var t in expert.Tags.Where(x => x.TopicState == (byte)TopicState.GroupTopic))
            {
                var tag = new TagViewModel(t);

                if(userInGroup == null)
                    AnonymouseTags.Add(tag);
                else if (expert.ExpertVotes.Any(x => x.ExpertId == expert.Id && x.GroupMemberId == userInGroup.Id && x.TagId == t.Id))
                    CheckedTags.Add(tag);
                else if (userInGroup.Expert == null)
                    NotCheckedTags.Add(tag);
                else
                {
                    var tt = DataService.PerThread.TagSet.Single(x => x.Id == t.Id);

                    if (!userInGroup.Expert.Tags.Contains(tt))
                        NotCheckedTags.Add(tag);
                    else
                        DisabledTags.Add(tag);
                }

                var votersCount = DataService.PerThread.ExpertVoteSet.Count(x => x.TagId == tag.Id && x.ExpertId == Expert.Id);
                VotersCountForTag.Add(tag.Id, votersCount);
            }
        }
    }

    public class UserExpertDelegate_ExpertViewModel
    {
        public string Avatar { get; set; }
        public Guid Id { get; set; }
        public string Info { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }

        public UserExpertDelegate_ExpertViewModel()
        {
        }

        public UserExpertDelegate_ExpertViewModel(Expert expert)
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
