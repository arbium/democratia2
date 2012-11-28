using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserGroupsViewModel
    {
        public IList<UserGroups_GroupMemberViewModel> GMs { get; set; }
        public string UserName { get; set; }
        public Guid UserId { get; set; }

        public UserGroupsViewModel()
        {
        }

        public UserGroupsViewModel(User user)
        {
            if (user != null)
            {
                GMs = new List<UserGroups_GroupMemberViewModel>();
                UserName = user.FullName;
                UserId = user.Id;

                foreach (var gm in user.GroupMembers.Where(x => x.State == (byte)GroupMemberState.Approved || x.State == (byte)GroupMemberState.Moderator || x.State == (byte)GroupMemberState.NotApproved).OrderByDescending(x => x.Group.GroupMembers.Count))
                    GMs.Add(new UserGroups_GroupMemberViewModel(gm));
            }
        }
    }
}