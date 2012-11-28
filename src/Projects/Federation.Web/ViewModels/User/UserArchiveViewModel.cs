using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserArchiveViewModel
    {
        public string FullName { get; set; }
        public Guid UserId { get; set; }

        public ContentTypeFilter ContentType { get; set; }
        public Personality Personality { get; set; }
        
        public _ContentFeedViewModel Feed { get; set; }
        public _CalendarViewModel Calendar { get; set; }
        
        public UserArchiveViewModel()
        {
            Feed = new _ContentFeedViewModel();
            Calendar = new _CalendarViewModel();
        }

        public UserArchiveViewModel(User user, int? year, int? month, int? day, ContentTypeFilter contentType, Personality personality)
        {
            Feed = new _ContentFeedViewModel();
            Calendar = new _CalendarViewModel();

            if (user != null)
            {
                FullName = user.FullName;
                UserId = user.Id;
                ContentType = contentType;
                Personality = personality;                

                if (contentType.IsSomethingChecked() && personality.IsSomethingChecked())
                {
                    IEnumerable<Content> content = user.Contents.Where(x => x.State == (byte)ContentState.Approved);
                    IEnumerable<Content> feed = null;

                    if (contentType.Posts)
                    {
                        IEnumerable<Post> posts = null;

                        if (personality.User && personality.Group)
                            posts = content.OfType<Post>();
                        else if (personality.User)
                            posts = content.OfType<Post>().Where(x => !x.GroupId.HasValue);
                        else if (personality.Group)
                            posts = content.OfType<Post>().Where(x => x.GroupId.HasValue);

                        if (feed == null)
                            feed = posts;
                        else
                            feed = feed.Union(posts);
                    }
                    if (contentType.Petitions)
                    {
                        IEnumerable<Petition> petitions = null;

                        if (personality.User && personality.Group)
                            petitions = content.OfType<Petition>();
                        else if (personality.User)
                            petitions = content.OfType<Petition>().Where(x => x.Group == null);
                        else if (personality.Group)
                            petitions = content.OfType<Petition>().Where(x => x.Group != null);

                        if (feed == null)
                            feed = petitions;
                        else
                            feed = feed.Union(petitions);
                    }

                    if (year.HasValue)
                        feed = feed.Where(x => x.PublishDate.Value.Year == year.Value);
                    if (month.HasValue)
                        feed = feed.Where(x => x.PublishDate.Value.Month == month.Value);
                    if (day.HasValue)
                        feed = feed.Where(x => x.PublishDate.Value.Day == day.Value);

                    Feed = new _ContentFeedViewModel(feed.OrderByDescending(x => x.PublishDate));
                    Calendar = new _CalendarViewModel(user, year, month, day);
                }                
            }
        }
    }
}