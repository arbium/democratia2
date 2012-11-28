using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupSearchViewModel
    {
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public string Tag { get; set; }

        public readonly _ContentFeedViewModel Feed = new _ContentFeedViewModel();

        public GroupSearchViewModel(string groupUrl, string tag)
        {
            var curGroup = GroupService.GetGroupByLabelOrId(groupUrl);

            GroupId = curGroup.Id;
            GroupUrl = curGroup.Url;
            GroupName = curGroup.Name;

            IEnumerable<Content> feed = null;
            Guid tagId;

            if (Guid.TryParse(tag, out tagId))
            {
                var t = DataService.PerThread.TagSet.SingleOrDefault(x => x.Id == tagId);
                if (t == null)
                    throw new BusinessLogicException("Указан неверный идентификатор темы");

                Tag = t.Title;

                feed = DataService.PerThread.ContentSet.Where(x => x.Tags.Any(y => y.Id == tagId) && x.State == (byte)ContentState.Approved).OrderByDescending(x=>x.PublishDate);
            }
            else if(!string.IsNullOrWhiteSpace(tag))
            {
                Tag = tag;
                tag = tag.ToLower();                

                feed = from content in DataService.PerThread.ContentSet.Where(x => x.GroupId == curGroup.Id && x.State == (byte)ContentState.Approved)
                                where content.Tags.Any(x => x.LowerTitle == tag)
                                orderby content.PublishDate descending
                                select content;                
            }

            Feed = new _ContentFeedViewModel(feed);
        }
    }
}
