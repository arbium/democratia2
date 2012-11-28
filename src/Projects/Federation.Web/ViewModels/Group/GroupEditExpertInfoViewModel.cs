using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupEditExpertInfoViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public string Info { get; set; }
        public string ReturnUrl { get; set; }

        public GroupEditExpertInfoViewModel()
        {
        }

        public GroupEditExpertInfoViewModel(Group group, Guid? userId)
        {
            if (group != null)
            {
                GroupId = group.Id;
                GroupUrl = group.Url;
                GroupName = group.Name;

                if (userId.HasValue)
                {
                    var uig = GroupService.UserInGroup(userId.Value, group.Id);

                    if (uig != null && uig.Expert != null)
                    {
                        Info = uig.Expert.Info;
                    }
                }
            }
        }
    }
}