using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupEditPostViewModel
    {
        private string _cleanText;

        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public bool IsContentModeration { get; set; }

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

        [Display(Name = "Темы")]
        public string TagTitles { get; set; }

        public GroupEditPostViewModel()
        {
        }

        public GroupEditPostViewModel(Post post)
        {
            PostId = post.Id;
            Title = post.Title;
            Text = post.Text;
            IsDraft = post.State == (byte)ContentState.Draft;
            TagTitles = TagsHelper.ConvertTagListToString(post.Tags);

            GroupId = post.Group.Id;
            GroupUrl = post.Group.Url;
            GroupName = post.Group.Name;
            IsContentModeration = post.Group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);
        }
    }
}