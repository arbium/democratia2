using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserVotersViewModel
    {
        public Guid UserId { get; set; }
        public bool IsForeignProfile { get; set; }
        public readonly IList<UserVoters_GroupViewModel> Groups = new List<UserVoters_GroupViewModel>();

        public UserVotersViewModel(User user, bool isForeignProfile)
        {
            IsForeignProfile = isForeignProfile;

            if (user == null)
                return;

            UserId = user.Id;
            
            Groups = user.GroupMembers
                .Where(x => x.Expert != null).Select(x => new UserVoters_GroupViewModel(x.Group, x.Expert, user.Id))
                .ToList();
        }
    }

    public class UserVoters_GroupViewModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public int VotersCount { get; set; }
        public string ExpertId { get; set; }

        public readonly IList<TagViewModel> ExpertTags = new List<TagViewModel>();

        public UserVoters_GroupViewModel(Group group, Expert expert, Guid userId)
        {
            if (group == null)
                return;

            Name = group.Name;
            Url = group.Url;
            Logo = ImageService.GetImageUrl<Group>(group.Logo);
            ExpertId = expert.Id.ToString();

            var expertGroupMember = GroupService.UserInGroup(userId, group);
            GroupMember userGroupMember = null;
            if(UserContext.Current != null)
                userGroupMember = GroupService.UserInGroup(UserContext.Current.Id, group);
            VotersCount = DataService.PerThread.ExpertVoteSet.Where(x => x.ExpertId == expert.Id).GroupBy(x => x.GroupMemberId).Count();
            ExpertTags = expertGroupMember.Expert.Tags.Select(x => new TagViewModel(x)).ToList();

            foreach (var tag in ExpertTags)
            {
                tag.IsChecked = false;

                if (userGroupMember!= null && userGroupMember.ExpertVotes.Count(x => x.TagId == tag.Id) > 0)
                    tag.IsChecked = true;
            }
            
        }
    }
}