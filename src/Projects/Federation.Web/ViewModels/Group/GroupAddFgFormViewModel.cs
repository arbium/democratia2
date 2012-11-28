using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupAddFgFormViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }

        public GroupAddFgFormViewModel()
        {
        }

        public GroupAddFgFormViewModel(Group group)
        {
            if (group != null)
            {
                GroupId = group.Id;
                GroupUrl = group.Url;
            }
        }
    }
}