using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _CalendarViewModel
    {
        public CalednarType Type { get; set; }

        public int? Day { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }

        public string Id { get; set; }
        public IList<DateTime> ContentDates { get; set; }

        public _CalendarViewModel()
        {
        }

        public _CalendarViewModel(DateTime today)
        {
            Type = CalednarType.Magazin;

            Day = today.Day;
            Month = today.Month;
            Year = today.Year;
        }

        public _CalendarViewModel(User user, int? year, int? month, int? day)
        {
            Type = CalednarType.Archive;

            Day = day;
            Month = month;            
            Year = year;

            ContentDates = new List<DateTime>();

            if (user != null)
            {
                Id = user.Id.ToString();

                foreach (Content content in user.Contents.Where(x => x.State == (byte)ContentState.Approved).ToList())
                    if (content.PublishDate.HasValue)
                        if (!ContentDates.Contains(content.PublishDate.Value))
                            ContentDates.Add(content.PublishDate.Value.Date);
            }
        }

        public _CalendarViewModel(Group group, int? year, int? month, int? day)
        {
            Type = CalednarType.Archive;

            Day = day;
            Month = month;
            Year = year;

            ContentDates = new List<DateTime>();

            if (group != null)
            {
                Id = group.Url;

                foreach (Post post in group.Content.OfType<Post>().Where(x => x.State == (byte)ContentState.Approved).ToList())
                    if (post.PublishDate.HasValue)
                        if (!ContentDates.Contains(post.PublishDate.Value))
                            ContentDates.Add(post.PublishDate.Value.Date);

                foreach (Voting voting in group.Content.OfType<Voting>().Where(x => x.State == (byte)ContentState.Approved).ToList())
                    if (voting.PublishDate.HasValue)
                        if (!ContentDates.Contains(voting.PublishDate.Value))
                            ContentDates.Add(voting.PublishDate.Value.Date);
            }
        }
    }

    public enum CalednarType
    {
        Archive,
        Magazin
    }
}