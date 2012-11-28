using System;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class User_SubscriptionViewModel
    {
        public readonly _ContentFeedViewModel LastContents = new _ContentFeedViewModel();

        public User_SubscriptionViewModel()
        {
        }

        public User_SubscriptionViewModel(User user, DateTime startDate, DateTime endDate)
        {
            if (user != null)
            {
                var groupContent = (from _group in user.SubscriptionGroups
                                  join content in DataService.PerThread.ContentSet on _group.Id equals content.GroupId
                                  where content.State == (byte)ContentState.Approved && startDate <= content.PublishDate && content.PublishDate <= endDate && !user.BlackList.Contains(content.Author)
                                  select content).ToList();

                var userContent = (from _user in user.SubscriptionUsers
                                 join content in DataService.PerThread.ContentSet on _user.Id equals content.AuthorId
                                 where content.State == (byte)ContentState.Approved && startDate <= content.PublishDate && content.PublishDate <= endDate
                                 select content).ToList();

                var feed = groupContent.Union(userContent).Distinct().Where(x => x.AuthorId != user.Id).OrderByDescending(c => c.PublishDate);

                LastContents = new _ContentFeedViewModel(feed, 10);
            }
        }
    }
}