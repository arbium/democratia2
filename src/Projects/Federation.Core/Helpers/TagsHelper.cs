using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace Federation.Core
{
    public static class TagsHelper
    {
        public static string[] SplitTagsString(string tags)
        { 
            string[] result = new string[0];

            if (!string.IsNullOrWhiteSpace(tags))
            {
                result = tags.Split(',');                
            }

            return result;
        }

        public static IList<Tag> ConvertStringToTagList(string tags, Group group)
        {
            IList<Tag> tagList = new List<Tag>();

            var tagsArr = SplitTagsString(tags);
            foreach (var tag in tagsArr)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    tagList.Add(TagService.GetTag(tag.Trim(), group, true));
            }

            return tagList;
        }

        public static IList<Tag> ConvertStringToTagList(string tags, Guid? groupId)
        {
            IList<Tag> tagList = new List<Tag>();

            var tagsArr = SplitTagsString(tags);
            foreach (var tag in tagsArr)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    tagList.Add(TagService.GetTag(tag.Trim(), groupId, true));
            }

            return tagList;
        }

        public static string ConvertTagListToString(ICollection<Tag> tags)
        {
            var builder = new StringBuilder();
            foreach (var tag in tags)
                builder.Append(tag.Title + ", ");

            if (builder.Length != 0)
                return builder.ToString().TrimEnd(new[] { ',', ' ' });

            return string.Empty;
        }

        public static Tag GetTagByTitle(string title)
        {
            return DataService.PerThread.TagSet.SingleOrDefault(x => x.LowerTitle == title.ToLower());
        }
    }
}