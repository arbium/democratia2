using System.Collections.Generic;

namespace Federation.Web.ViewModels
{
    public class _RssFeedViewModel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PublishDate { get; set; }

        public List<_RssFeedItemViewModel> Items { get; set; }

        public _RssFeedViewModel()
        {
            Items = new List<_RssFeedItemViewModel>();
        }
    }
}