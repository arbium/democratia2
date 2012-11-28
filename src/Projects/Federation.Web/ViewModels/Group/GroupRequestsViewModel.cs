using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupRequestsViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public bool GroupIsBlank { get; set; }
        public readonly IList<GroupRequestsViewModel_RequestViewModel> Requests = new List<GroupRequestsViewModel_RequestViewModel>();

        public GroupRequestsViewModel()
        {
        }

        public GroupRequestsViewModel(Group group)
        {
            if (group != null)
            {
                GroupId = group.Id;
                GroupUrl = group.Url;
                GroupName = group.Name;
                GroupIsBlank = group.State == (byte)GroupState.Archive || group.State == (byte)GroupState.Blank;

                foreach (var member in group.GroupMembers.Where(x => x.State == (byte) GroupMemberState.NotApproved))
                    Requests.Add(new GroupRequestsViewModel_RequestViewModel(member));
            }
        }
    }

    public class GroupRequestsViewModel_RequestViewModel
    {
        public Guid Id { get; set; }
        public Guid GroupMemberId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        
        public GroupRequestsViewModel_RequestViewModel()
        {
        }

        public GroupRequestsViewModel_RequestViewModel(GroupMember member)
        {
            if (member != null)
            {
                Id = member.UserId;
                GroupMemberId = member.Id;
                FullName = member.User.FullName;
                Avatar = ImageService.GetImageUrl<User>(member.User.Avatar);
            }
        }
    }
}