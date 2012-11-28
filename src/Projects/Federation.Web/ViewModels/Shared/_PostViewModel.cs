using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _PostViewModel
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public _PostViewModel()
        {
        }

        public _PostViewModel(Post post)
        {
            if (post != null)
            {
                Title = post.Title;
                Text = post.Text;
            }
        }
    }
}