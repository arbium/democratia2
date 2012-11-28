using System;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class SearchTag_GroupViewModel
    {
        public string Link { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public int MembersCount { get; set; }
        public DateTime CreationDate { get; set; }

        public SearchTag_GroupViewModel()
        {
        }

        public SearchTag_GroupViewModel(Group group)
        {
            if (group != null)
            {
                Link = UrlHelper.GetUrl<Group>(group.Url);
                Logo = ImageService.GetImageUrl<Group>(group.Logo);
                Name = group.Name;
                MembersCount = group.GroupMembers.Count(x => x.State == (byte) GroupMemberState.Approved || x.State == (byte) GroupMemberState.Moderator);
                CreationDate = group.CreationDate;
            }
        }
    }
}