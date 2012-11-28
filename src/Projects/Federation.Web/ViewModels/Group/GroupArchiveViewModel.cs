using System.Collections.Generic;
using System.Linq;
using Federation.Core;
using System;

namespace Federation.Web.ViewModels
{
    public class GroupArchiveViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }

        public ContentTypeFilter ContentType { get; set; }

        public _ContentFeedViewModel Feed { get; set; }
        public _CalendarViewModel Calendar { get; set; }
        
        public GroupArchiveViewModel()
        {
            Feed = new _ContentFeedViewModel();
            Calendar = new _CalendarViewModel();
        }

        public GroupArchiveViewModel(Group group, int? year, int? month, int? day, ContentTypeFilter contentType)
        {
            Feed = new _ContentFeedViewModel();
            Calendar = new _CalendarViewModel();

            if (group != null)
            {
                GroupId = group.Id;
                GroupName = group.Name;
                GroupUrl = group.Url;
                ContentType = contentType;

                if (contentType.IsSomethingChecked())
                {
                    IEnumerable<Content> content1 = group.Content.OfType<Post>().Where(x => x.State == (byte)ContentState.Approved);
                    IEnumerable<Content> content2 = group.Content.OfType<Voting>().Where(x => x.State == (byte)ContentState.Approved);
                    IEnumerable<Content> content = content1.Union(content2);
                    IEnumerable<Content> feed = null;

                    if (contentType.Posts)
                    {
                        IEnumerable<Post> posts = content.OfType<Post>();

                        if (feed == null)
                            feed = posts;
                        else
                            feed = feed.Union(posts);
                    }
                    if (contentType.Polls)
                    {
                        IEnumerable<Poll> polls = content.OfType<Poll>();

                        if (feed == null)
                            feed = polls;
                        else
                            feed = feed.Union(polls);
                    }
                    if (contentType.Petitions)
                    {
                        IEnumerable<Petition> petitions = content.OfType<Petition>();

                        if (feed == null)
                            feed = petitions;
                        else
                            feed = feed.Union(petitions);
                    }
                    if (contentType.Elections)
                    {
                        IEnumerable<Election> elections = content.OfType<Election>();

                        if (feed == null)
                            feed = elections;
                        else
                            feed = feed.Union(elections);
                    }

                    if (year.HasValue)
                        feed = feed.Where(x => x.PublishDate.Value.Year == year.Value);
                    if (month.HasValue)
                        feed = feed.Where(x => x.PublishDate.Value.Month == month.Value);
                    if (day.HasValue)
                        feed = feed.Where(x => x.PublishDate.Value.Day == day.Value);

                    Feed = new _ContentFeedViewModel(feed.OrderByDescending(x => x.PublishDate));
                    Calendar = new _CalendarViewModel(group, year, month, day);
                }
            }
        }
    }
}