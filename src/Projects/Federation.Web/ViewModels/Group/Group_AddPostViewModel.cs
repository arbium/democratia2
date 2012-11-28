using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupAddPostViewModel
    {
        private string _cleanText;

        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public bool IsContentModeration { get; set; }

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

        [Display(Name = "Внешний пост")]
        public bool IsExternal { get; set; }

        [Display(Name = "Темы")]
        public string TagTitles { get; set; }

        public GroupAddPostViewModel()
        {
        }

        public GroupAddPostViewModel(Group group = null)
        {
            if (group != null)
            {
                GroupId = group.Id;
                GroupUrl = group.Url;
                GroupName = group.Name;
                IsContentModeration = group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);
            }
        }
    }
}