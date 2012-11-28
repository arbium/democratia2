using System;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserBlogViewModel
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public bool IsPMAllow { get; set; }

        public readonly _ContentFeedViewModel Feed = new _ContentFeedViewModel();

        public UserBlogViewModel()
        {
        }

        public UserBlogViewModel(User user, Guid? currentUid = null)
        {
            if (user == null)
                return;

            UserId = user.Id;
            UserFullName = user.FullName;

            if (currentUid.HasValue && user.BlackList.Count(x => x.Id == currentUid.Value) == 0)
                IsPMAllow = true;

            var feed = user.Contents.Where(x => x.State == (byte)ContentState.Approved && x.PublishDate.HasValue).OrderByDescending(x => x.PublishDate);
            Feed = new _ContentFeedViewModel(feed, user.Id);
        }
    }
}
