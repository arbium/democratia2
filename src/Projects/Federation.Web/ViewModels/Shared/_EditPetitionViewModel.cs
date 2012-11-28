using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _EditPetitionViewModel
    {
        private string _cleanText;

        public Guid Id { get; set; }
        public Guid? GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public Guid? AuthorId { get; set; }
        public string AuthorName { get; set; }

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

        [Display(Name = "Список тем (через запятую)")]
        public string TagTitles { get; set; }

        public _EditPetitionViewModel()
        {
        }

        public _EditPetitionViewModel(Petition petition)
        {
            Id = petition.Id;
            GroupId = petition.GroupId;
            if (GroupId.HasValue)
            {
                GroupUrl = petition.Group.Url;
                GroupName = petition.Group.Name;
            }
            AuthorId = petition.AuthorId;
            if (AuthorId.HasValue)
            {
                AuthorName = petition.Author.SurName + " " + petition.Author.FirstName + " " + petition.Author.Patronymic;
            }
            Title = petition.Title;
            Text = petition.Text;
            IsPrivate = petition.IsPrivate;
            TagTitles = TagsHelper.ConvertTagListToString(petition.Tags);
        }
    }
}