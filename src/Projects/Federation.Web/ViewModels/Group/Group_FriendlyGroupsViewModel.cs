using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_FriendlyGroupsViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }

        public readonly List<Group_FriendlyGroups_GroupViewModel> Groups = new List<Group_FriendlyGroups_GroupViewModel>();

        public Group_FriendlyGroupsViewModel()
        {
        }

        public Group_FriendlyGroupsViewModel(Guid groupId)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == groupId);
            if (group == null)
                throw new BusinessLogicException("Указан неверный идентификатор группы");

            GroupId = groupId;
            GroupUrl = group.Url;

            Groups = group.FriendlyGroups.Select(x => new Group_FriendlyGroups_GroupViewModel(x)).ToList();
        }
    }

    public class Group_FriendlyGroups_GroupViewModel
    {
        public Guid Id { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public Group_FriendlyGroups_GroupViewModel()
        {
        }

        public Group_FriendlyGroups_GroupViewModel(Group group)
        {
            if (group != null)
            {
                Id = group.Id;
                Logo = ImageService.GetImageUrl<Group>(group.Logo);
                Name = group.Name;
                Url = group.Url;
            }
        }
    }
}

