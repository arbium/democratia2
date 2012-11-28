using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_RecommendedGroupsViewModel
    {
        public Guid GroupId { get; set; }
        public readonly List<Group_RecommendedGroups_GroupViewModel> Groups = new List<Group_RecommendedGroups_GroupViewModel>();

        public Group_RecommendedGroupsViewModel()
        {
        }

        public Group_RecommendedGroupsViewModel(Guid groupId, int amount = 3)
        {
            var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == groupId);
            if (group == null)
                throw new BusinessLogicException("Указан неверный идентификатор группы");

            GroupId = groupId;

            foreach (var gt in group.Tags.Where(x => x.TopicState == (byte)TopicState.GroupTopic))
            {
                var tags = DataService.PerThread.TagSet.Where(x => x.TopicState == (byte)TopicState.GroupTopic && x.LowerTitle == gt.LowerTitle && x.GroupId != groupId);
                foreach (var tag in tags)
                    Groups.Add(new Group_RecommendedGroups_GroupViewModel(tag.Group));
            }

            if (UserContext.Current == null)
                Groups = Groups.Distinct().Take(amount).ToList();
            else
                Groups = Groups.Distinct().Where(x => !UserContext.Current.IsUserInGroup(x.Id)).Take(amount).ToList();            
        }
    }

    public class Group_RecommendedGroups_GroupViewModel
    {
        public Guid Id { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public Group_RecommendedGroups_GroupViewModel()
        {
        }

        public Group_RecommendedGroups_GroupViewModel(Group group)
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

