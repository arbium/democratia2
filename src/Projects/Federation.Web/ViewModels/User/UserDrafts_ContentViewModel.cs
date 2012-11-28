using System;
using System.Collections.Generic;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserDrafts_ContentViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime PostDate { get; set; }
        public string Controller { get; set; }
        public Guid? AuthorId { get; set; }

        public IList<TagViewModel> Tags { get; set; }

        public UserDrafts_ContentViewModel()
        {
        }

        public UserDrafts_ContentViewModel(Content content)
        {
            if (content != null)
            {
                Id = content.Id;
                Title = content.Title;
                Summary = content.Text.Substring(0, Math.Min(ConstHelper.SummaryLength, content.Text.Length));
                AuthorId = content.AuthorId;
                PostDate = content.CreationDate;
                Controller = content.Controller;

                Summary = TextHelper.CleanTags(content.Text);
                if (Summary.Length > ConstHelper.MiniSummaryLength)
                    Summary = Summary.Substring(0, ConstHelper.MiniSummaryLength) + "…";

                Tags = new List<TagViewModel>();
                foreach(var tag in content.Tags)
                    Tags.Add(new TagViewModel(tag));
            }
        }

    }
}