using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class HomeIndexViewModel : CachedViewModel
    {
        public _ContentFeedViewModel Recent { get; set; }
        public Home_PopularViewModel Popular { get; set; }

        public HomeIndexViewModel()
        {
			Recent = new _ContentFeedViewModel();
            Popular = new Home_PopularViewModel();
        }

        public override ICachedViewModel Initialize()
        {
            var groups = DataService.PerThread.GroupSet.OrderByDescending(x => 5 * x.GroupMembers.Count(gm => gm.State == (byte)GroupMemberState.Approved || gm.State == (byte)GroupMemberState.Moderator) + 2 * x.Content.OfType<Post>().Count(p => p.State == (byte)ContentState.Approved)).Where(x => x.State != (byte)GroupState.Blank && x.State != (byte)GroupState.Archive).Take(20).ToList();
            var users = DataService.PerThread.BaseUserSet.OfType<User>().OrderByDescending(x => 5 * x.Subscribers.Count + 2 * x.Contents.OfType<Post>().Count(p => p.State == (byte)ContentState.Approved)).Take(20).ToList();
            /*var yesterday = DateTime.Now.AddDays(-2);
            var recent = DataService.PerThread.ContentSet.OfType<Post>()
                .Where(x => x.PublishDate.HasValue && x.State == (byte)ContentState.Approved)
                .Where(x => x.PublishDate.Value <= DateTime.Now && x.PublishDate.Value > yesterday)
                .Where(x => !x.GroupId.HasValue || (x.Group.State != (byte)GroupState.Blank || x.Group.State != (byte)GroupState.Archive))
                .OrderByDescending(x => 2 * x.Comments.Count + 5 * (x.Likes.Count(a => a.Value) - x.Likes.Count(a => !a.Value)) + x.Likes.Count)
                .Take(10).ToList().OfType<Content>().ToList();*/
            var rootGroup = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == ConstHelper.RootGroupId);
            IEnumerable<Content> recent = new List<Content>();
            if (rootGroup != null)
            {
                List<Content> content = new List<Content>();

                var polls = rootGroup.Content.OfType<Poll>().Where(x => x.State == (byte)ContentState.Approved);
                var surveys = rootGroup.Content.OfType<Survey>().Where(x => x.State == (byte)ContentState.Approved);

                content.AddRange(polls);
                content.AddRange(surveys);

                recent = content.OrderByDescending(x => x.PublishDate);
            }

            Recent = new _ContentFeedViewModel(recent);            
            Popular = new Home_PopularViewModel(groups, users);

			return this;
        }
    }
}