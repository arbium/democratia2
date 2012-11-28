using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class TagViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Weight { get; set; }
        public bool IsRecomended { get; set; }
        public bool IsChecked { get; set; }
        public bool IsEnabled { get; set; }

        public TagViewModel()
        {
        }

        public TagViewModel(Tag tag)
        {
            if (tag != null)
            {
                Id = tag.Id;
                Title = tag.Title;
                IsRecomended = tag.IsRecommended;
                Description = tag.Description;
                Weight = tag.Weight;
            }
        }
    }
}