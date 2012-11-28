using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserEditPostViewModel
    {
        private string _cleanText;

        public Guid PostId { get; set; }

        [Display(Name = "Заголовок")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Title { get; set; }

        [Display(Name = "Текст")]
        [Required(ErrorMessage = "Обязательное поле")]
        [AllowHtml]
        public string Text
        {
            get { return _cleanText; }
            set { _cleanText = TextHelper.CleanXssTags(HttpUtility.HtmlDecode(value)); }
        }

        [Display(Name = "Черновик")]
        public bool IsDraft { get; set; }

        [Display(Name = "Тэги")]
        public string TagTitles { get; set; }

        public UserEditPostViewModel()
        {
        }

        public UserEditPostViewModel(Post post)
        {
            PostId = post.Id;
            Title = post.Title;
            Text = post.Text;
            IsDraft = post.State == (byte)ContentState.Draft;
            TagTitles = TagsHelper.ConvertTagListToString(post.Tags);
        }
    }
}