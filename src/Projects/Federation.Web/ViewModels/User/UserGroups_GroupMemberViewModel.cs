using System;
using System.Linq;
using System.Collections.Generic;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserGroups_GroupMemberViewModel
    {
        public DateTime EntryDate { get; set; }
        public string GroupLogo { get; set; }
        public string GroupName { get; set; }
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public bool IsApproved { get; set; }
        public bool IsModerator { get; set; }
        public int MembersCount { get; set; }

        public readonly IList<TagViewModel> Tags = new List<TagViewModel>();
        
        public UserGroups_GroupMemberViewModel()
        {
        }

        public UserGroups_GroupMemberViewModel(GroupMember gm)
        {
            if (gm != null)
            {
                EntryDate = gm.EntryDate;
                GroupLogo = ImageService.GetImageUrl<Group>(gm.Group.Logo);
                GroupName = gm.Group.Name;
                GroupId = gm.Group.Id;
                GroupUrl = gm.Group.Url;
                MembersCount = gm.Group.GroupMembers.Count;

                IsApproved = gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator;
                IsModerator = gm.State == (byte)GroupMemberState.Moderator;
                
                foreach (var tag in gm.Group.Tags.Where(x => x.TopicState == (byte)TopicState.GroupTopic))
                    Tags.Add(new TagViewModel(tag));
            }
        }
    }
}