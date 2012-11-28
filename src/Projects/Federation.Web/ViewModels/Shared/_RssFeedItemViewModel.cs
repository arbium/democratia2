
using Federation.Core;
namespace Federation.Web.ViewModels
{
    public class _RssFeedItemViewModel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PublishDate { get; set; }

        public _RssFeedItemViewModel()
        {
        }

        public _RssFeedItemViewModel(Content content)
        {
            Title = content.Title;
            Link = content.GetUrl(false);
            Description = content.Text;
            PublishDate = content.PublishDate.Value.ToString("dd MMMM yyyy"); ;
        }
    }
}
