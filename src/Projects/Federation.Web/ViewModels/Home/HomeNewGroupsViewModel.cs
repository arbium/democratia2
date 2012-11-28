using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class HomeGroupsViewModel : CachedViewModel
    {
        public IList<HomeGroups_GroupViewModel> FilteredGroups { get; set; }
        public int GroupStateFilter { get; set; }
        public int? OrderByFilter { get; set; }

        public HomeGroupsViewModel(GroupState? groupState, GroupOrderByType? orderBy)
        {
            GroupStateFilter = groupState.HasValue ? (byte)groupState : -1;
            OrderByFilter = (byte)(orderBy.HasValue ? orderBy.Value : GroupOrderByType.ByTitle);
            CachLabel = GroupStateFilter + "&" + OrderByFilter;
            CachDropPeriod = TimeSpan.FromHours(12);
        }

        public override ICachedViewModel Initialize()
        {
            FilteredGroups = new List<HomeGroups_GroupViewModel>();
			var groupsQuery = DataService.PerThread.GroupSet.Where(x=>true);
            if (GroupStateFilter != -1)
                groupsQuery = groupsQuery.Where(g => g.State == GroupStateFilter);

			switch ((GroupOrderByType)OrderByFilter)
            {
                case GroupOrderByType.ByCreationDate:
                    groupsQuery = groupsQuery.OrderByDescending(x => x.CreationDate);
                    break;

                case GroupOrderByType.ByMembersCount:
                    groupsQuery = groupsQuery                        
                        .OrderByDescending(x => x.GroupMembers.Count(gm => gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator));
                    break;

                case GroupOrderByType.ByTitle:
                    groupsQuery = groupsQuery.OrderBy(x => x.Name);
                    break;
            }

            foreach (Group child in groupsQuery.ToList())
                FilteredGroups.Add(new HomeGroups_GroupViewModel(child));
			return this;
        }
    }

    public enum GroupOrderByType
    {
        ByMembersCount,
        ByCreationDate,
        ByTitle
    }

    public class HomeGroups_GroupViewModel
    {
        public Guid Id { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int MembersCount { get; set; }
        public IList<TagViewModel> Tags { get; set; }

        public HomeGroups_GroupViewModel()
        {
        }

        public HomeGroups_GroupViewModel(Group group)
        {
            if (group != null)
            {
                Id = group.Id;
                Logo = ImageService.GetImageUrl<Group>(group.Logo);
                Name = group.Name;
                Url = group.Url;
                MembersCount = group.GroupMembers.Count(gm => gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator);

                Tags = new List<TagViewModel>();
                foreach (var tag in group.Tags.Where(x=>x.TopicState == (byte)TopicState.GroupTopic).OrderByDescending(x=>x.Weight).Take(10))
                    Tags.Add(new TagViewModel(tag));
            }
        }
    }
}