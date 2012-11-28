using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _CreatePetitionViewModel
    {
        private string _cleanText;

        public Guid? GroupId { get; set; }
        public string GroupIdString
        {
            get
            {
                if (GroupId.HasValue)
                    return GroupId.ToString();

                return string.Empty;
            }
        }

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

        [Display(Name = "Только для членов группы")]
        public bool IsPrivate { get; set; }

        [Display(Name = "Тэги (через запятую)")]
        public string TagTitles { get; set; }
    }
}