using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class HomeTodayViewModel : CachedViewModel
    {
        public DateTime Today { get; set; }
        public List<HomeToday_RecordViewModel> All { get; set; }
        public List<HomeToday_RecordViewModel> Popular { get; set; }        
        public List<HomeToday_RecordViewModel> Federation { get; set; }
        public List<HomeToday_RecordViewModel> ImportantGroups { get; set; }

        public HomeTodayViewModel()
        {
        }

        public HomeTodayViewModel(DateTime date)
        {
            Today = date.Date;
            CachDropPeriod = new TimeSpan(1, 0, 0);
            CachLabel = Today.ToString("ddMMyy");
        }

        public override ICachedViewModel  Initialize()
        {
            var actual = Today.Date.AddDays(-3);            
            var end = Today.Date.AddDays(1);

            All = new List<HomeToday_RecordViewModel>();
            Popular = new List<HomeToday_RecordViewModel>();
            Federation = new List<HomeToday_RecordViewModel>();
            ImportantGroups = new List<HomeToday_RecordViewModel>();
            ICollection<Guid> published = new HashSet<Guid>();

            var allFeed = DataService.PerThread.ContentSet.OfType<Content>().Where(x => x.PublishDate.HasValue && x.State == (byte)ContentState.Approved).Where(x => x.PublishDate.Value >= actual && x.PublishDate.Value < end).OrderByDescending(x => (12 * x.Likes.Sum(l => l.Value ? 1 : -1) + x.Comments.Count + 1) * (Today > x.PublishDate ? 1 : 3)).Take(50);
            var grouped = allFeed.Where(x => x.GroupId.HasValue && x.GroupId.Value != ConstHelper.RootGroupId).GroupBy(x => x.GroupId);

            var importantGroupsFeed = new List<Content>();
            foreach(var item in grouped)
            {
                item.OrderByDescending(x => (12 * x.Likes.Sum(l => l.Value?1:-1) + x.Comments.Count + 1) * (Today > x.PublishDate ? 1 : 3));
                if(item.Any())
                    importantGroupsFeed.Add(item.First());
            }
            
            var federationFeed = allFeed.Where(x => x.GroupId.HasValue && x.GroupId.Value == ConstHelper.RootGroupId).Take(3);

            importantGroupsFeed = importantGroupsFeed.OrderByDescending(x => (12 * x.Likes.Sum(l => l.Value ? 1 : -1) + x.Comments.Count + 1) * (Today > x.PublishDate ? 1 : 3)).Take(3).ToList();
            var excludeList = importantGroupsFeed.Select(x => x.Id).ToList();
            excludeList.AddRange(federationFeed.ToList().Select(x => x.Id));
            var tempFeed = allFeed.Where(x => !excludeList.Contains(x.Id));

            ImportantGroups = importantGroupsFeed.Select(x => new HomeToday_RecordViewModel(x)).ToList();
            Federation = federationFeed.ToList().Select(x => new HomeToday_RecordViewModel(x)).ToList();
            Popular = tempFeed.Take(10).ToList().Select(x => new HomeToday_RecordViewModel(x)).ToList();
            All = tempFeed.Skip(10).ToList().Select(x => new HomeToday_RecordViewModel(x)).ToList();        

 	        return this;
        }            
    }    

    public class HomeToday_RecordViewModel
    {
        public Guid Id { get; set; }
        public ContentViewType ContentType { get; set; }
        public string Url { get; set; }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Summary { get; set; }
        public int CommentsCount { get; set; }
        public string CommentsString { get; set; }        

        public Guid? AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorSurname { get; set; }
        public string AuthorAvatar { get; set; }

        public Guid? GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupLogo { get; set; }

        public HomeToday_RecordViewModel()
        {
        }

        public HomeToday_RecordViewModel(Content content)
        {
            if (content != null)
            {
                Id = content.Id;
                Url = content.GetUrl();
                Title = content.Title;
                Summary = TextHelper.CleanTags(content.Text);
                if (Summary.Length > ConstHelper.MiniSummaryLength)
                    Summary = Summary.Substring(0, ConstHelper.MiniSummaryLength) + "...";
                CommentsCount = content.Comments.Count(c => !c.IsHidden);
                CommentsString = DeclinationService.OfNumber(CommentsCount, "комментарий", "комментария", "комментариев");
                Date = content.CreationDate;

                if (content.AuthorId.HasValue)
                {
                    AuthorId = content.AuthorId.Value;
                    AuthorName = content.Author.FirstName;
                    AuthorSurname = content.Author.SurName;
                    AuthorAvatar = ImageService.GetImageUrl<User>(content.Author.Avatar);
                }

                if (content.GroupId.HasValue)
                {
                    GroupId = content.GroupId.Value;
                    GroupName = content.Group.Name;
                    GroupLogo = ImageService.GetImageUrl<Group>(content.Group.Logo);

                    ContentType = ContentViewType.GroupPost;
                }
                else
                    ContentType = ContentViewType.UserPost;
            }
        }
    }
}