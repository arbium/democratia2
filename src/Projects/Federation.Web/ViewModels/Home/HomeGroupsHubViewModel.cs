using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class HomeGroupsHubViewModel : CachedViewModel
    {
        public IList<HomeGroupsHub_CategoryViewModel> Categories { get; private set; }

        public IList<HomeGroupsHub_ItemViewModel> GroupAds { get; private set; }
        public IList<HomeGroupsHub_ItemViewModel> TopFilteredGroups { get; private set; }
        public IList<HomeGroupsHub_ItemViewModel> RndFilteredGroups { get; private set; }
        public IList<HomeGroupsHub_ItemViewModel> NewGroups { get; private set; }

        public HomeGroupsHubViewModel(string filter = null)
        {
            var categories = DataService.PerThread.GroupCategorySet.ToList();

            var totalGroups = DataService.PerThread.GroupSet
                .Count(x => x.State != (byte)GroupState.Archive && x.State != (byte)GroupState.Blank && x.GroupRating.OverallRating != 0);
            var filteredGroups = DataService.PerThread.GroupSet
                .Where(x => x.State != (byte)GroupState.Archive && x.State != (byte)GroupState.Blank && x.GroupRating.OverallRating != 0);

            if (!string.IsNullOrEmpty(filter))
            {
                IQueryable<Group> tmpGroups = null;
                for (var i = 0; i < categories.Count; i++)
                    if (filter.Contains(i.ToString(CultureInfo.InvariantCulture)))
                    {
                        var cat = categories[i];

                        if (tmpGroups == null)
                            tmpGroups = filteredGroups.Where(x => x.Categories.Count(y => y.Id == cat.Id) != 0);
                        else
                            tmpGroups = tmpGroups.Union(filteredGroups.Where(x => x.Categories.Count(y => y.Id == cat.Id) != 0));
                    }
                filteredGroups = tmpGroups;
            }

            GroupAds = DataService.PerThread.GroupAdSet
                .Where(x => x.IsActive).ToList().Select(x => new HomeGroupsHub_ItemViewModel(x.Group, totalGroups))
                .OrderBy(x => Guid.NewGuid()).Take(3).ToList();

            if (filteredGroups != null)
            {
                TopFilteredGroups = filteredGroups.OrderBy(x => x.GroupRating.OverallRating).Take(5).ToList()
                    .Select(x => new HomeGroupsHub_ItemViewModel(x, totalGroups)).ToList();
                RndFilteredGroups = filteredGroups.OrderBy(x => x.GroupRating.OverallRating).Skip(5).OrderBy(x => Guid.NewGuid()).Take(5).ToList()
                    .Select(x => new HomeGroupsHub_ItemViewModel(x, totalGroups)).ToList();
            }

            NewGroups = DataService.PerThread.GroupSet
                .OrderByDescending(x => x.CreationDate)
                .ThenBy(x => x.GroupMembers.Count(m => m.State == (byte)GroupMemberState.Approved || m.State == (byte)GroupMemberState.Moderator))
                .Take(10).ToList().Select(x => new HomeGroupsHub_ItemViewModel(x, totalGroups)).ToList();

            Categories = categories.Select(x => new HomeGroupsHub_CategoryViewModel(x)).ToList();
        }
    }

    public class HomeGroupsHub_CategoryViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public HomeGroupsHub_CategoryViewModel()
        {
        }

        public HomeGroupsHub_CategoryViewModel(GroupCategory category)
        {
            Id = category.Id;
            Title = category.Title;
            Description = category.Description;
        }
    }

    public class HomeGroupsHub_ItemViewModel
    {
        public Guid Id { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }

        public long RatingPosition { get; set; }
        public long OldPosition { get; set; }

        public byte MembersRating { get; set; }
        public byte ExpertsRating { get; set; }
        public byte ContentRating { get; set; }

        public string AdText { get; set; }

        public HomeGroupsHub_ItemViewModel()
        {
        }

        public HomeGroupsHub_ItemViewModel(Group group, int totalGroups)
        {
            Id = group.Id;
            Logo = ImageService.GetImageUrl<Group>(group.Logo);
            Name = group.Name;
            Url = group.Url;
            Summary = TextHelper.CleanTags(group.Summary);
            if (Summary.Length > ConstHelper.MiniSummaryLength)
                Summary = Summary.Substring(0, ConstHelper.MiniSummaryLength) + "...";

            RatingPosition = group.GroupRating.OverallRating;
            OldPosition = group.GroupRating.OldOverallRating;

            MembersRating = (byte)(5 * (float)(totalGroups - group.GroupRating.MembersRating + 1) / totalGroups);
            ExpertsRating = (byte)(5 * (float)(totalGroups - group.GroupRating.ExpertsRating + 1) / totalGroups);
            ContentRating = (byte)(5 * (float)(totalGroups - group.GroupRating.ContentRating + 1) / totalGroups);

            if (group.GroupAd != null)
                AdText = group.GroupAd.Text;
        }
    }
}