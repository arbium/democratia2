using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupChildrenViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public IList<GroupChildren_GroupViewModel> FormedGroups { get; set; }
        public IList<GroupChildren_GroupViewModel> BlankGroups { get; set; }

        public GroupChildrenViewModel()
        {
            FormedGroups = new List<GroupChildren_GroupViewModel>();
            BlankGroups = new List<GroupChildren_GroupViewModel>();
        }

        public GroupChildrenViewModel(Group group)
        {
            FormedGroups = new List<GroupChildren_GroupViewModel>();
            BlankGroups = new List<GroupChildren_GroupViewModel>();

            if (group != null)
            {
                GroupId = group.Id;
                GroupUrl = group.Url;
                GroupName = group.Name;

                foreach (var child in group.ChildGroups.Where(g => g.State != (byte)GroupState.Blank && g.State != (byte)GroupState.Archive).OrderByDescending(g => g.GroupMembers.Count))
                    FormedGroups.Add(new GroupChildren_GroupViewModel(child));

                foreach (var child in group.ChildGroups.Where(g => g.State == (byte)GroupState.Blank || g.State == (byte)GroupState.Archive).OrderByDescending(g => g.GroupMembers.Count))
                    BlankGroups.Add(new GroupChildren_GroupViewModel(child));
            }
        }
    }

    public class GroupChildren_GroupViewModel
    {
        public Guid Id { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int MembersCount { get; set; }
        public IList<TagViewModel> Tags { get; set; }

        public GroupChildren_GroupViewModel()
        {
        }

        public GroupChildren_GroupViewModel(Group group)
        {
            if (group != null)
            {
                Id = group.Id;
                Logo = ImageService.GetImageUrl<Group>(group.Logo);
                Name = group.Name;
                Url = group.Url;
                MembersCount = group.GroupMembers.Count(gm => gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator);

                Tags = new List<TagViewModel>();
                foreach (var tag in group.Tags)
                    Tags.Add(new TagViewModel(tag));
            }
        }
    }
}