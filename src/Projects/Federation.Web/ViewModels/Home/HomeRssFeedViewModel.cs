using System.Collections.Generic;
using Federation.Core;
using System;
using System.Linq;

namespace Federation.Web.ViewModels
{
    public class HomeRssFeedViewModel : CachedViewModel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PublishDate { get; set; }

        public List<_RssFeedItemViewModel> Items { get; set; }

        public HomeRssFeedViewModel()
        {
            CachDropPeriod = new TimeSpan(12, 0, 0);
        }

        public override ICachedViewModel Initialize()
        {
            Items = new List<_RssFeedItemViewModel>();

            for (int i = 0; i < 30; i++)
            {
                Items.AddRange(GetContentForDay(DateTime.Now.AddDays(-i)).Select(x=>new _RssFeedItemViewModel(x)).ToList());
            }
            
            return this;
        }

        public IList<Content> GetContentForDay(DateTime today)
        {
            List<Content> result = new List<Content>();
            DateTime actual = today.Date.AddDays(-3);
            DateTime end = today.Date.AddDays(1);

            ICollection<Guid> published = new HashSet<Guid>();

            var allFeed = DataService.PerThread.ContentSet.OfType<Content>().Where(x => x.PublishDate.HasValue && x.State == (byte)ContentState.Approved).Where(x => x.PublishDate.Value >= actual && x.PublishDate.Value < end).OrderByDescending(x => (12 * x.Likes.Sum(l => l.Value ? 1 : -1) + x.Comments.Count + 1) * (today > x.PublishDate ? 1 : 3)).Take(50);
            var grouped = allFeed.Where(x => x.GroupId.HasValue && x.GroupId.Value != ConstHelper.RootGroupId).GroupBy(x => x.GroupId);

            var totalCount = allFeed.Count();

            List<Content> importantGroupsFeed = new List<Content>();
            foreach (var item in grouped)
            {
                item.OrderByDescending(x => (12 * x.Likes.Sum(l => l.Value ? 1 : -1) + x.Comments.Count + 1) * (today > x.PublishDate ? 1 : 3));
                if(item.Count()>0)
                    importantGroupsFeed.Add(item.First());
            }

            var federationFeed = allFeed.Where(x => x.GroupId.HasValue && x.GroupId.Value == ConstHelper.RootGroupId).Take(3);

            importantGroupsFeed = importantGroupsFeed.OrderByDescending(x => (12 * x.Likes.Sum(l => l.Value ? 1 : -1) + x.Comments.Count + 1) * (today > x.PublishDate ? 1 : 3)).Take(3).ToList();
            var excludeList = importantGroupsFeed.Select(x => x.Id).ToList();
            excludeList.AddRange(federationFeed.ToList().Select(x => x.Id));
            var tempFeed = allFeed.Where(x => !excludeList.Contains(x.Id));

            result.AddRange((federationFeed.Where(x => today <= x.PublishDate.Value && x.PublishDate.Value < end)).ToList());
            result.AddRange((importantGroupsFeed.Where(x => today <= x.PublishDate.Value && x.PublishDate.Value < end)).ToList());
            result.AddRange((tempFeed.Take(10).Where(x => today <= x.PublishDate.Value && x.PublishDate.Value < end)).ToList());

            return result;
        }
    }
}