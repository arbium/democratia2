using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupSurveyOptionViewModel
    {
        private string _cleanText;

        public Guid Id { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        [AllowHtml]
        public string Description
        {
            get { return _cleanText; }
            set { _cleanText = TextHelper.CleanXssTags(HttpUtility.HtmlDecode(value)); }
        }

        public string VotesCountString { get; set; }
        public int VotesCount { get; set; }
        public bool IsChecked { get; set; }
        public byte Position { get; set; }

        public GroupSurveyOptionViewModel() { }

        public GroupSurveyOptionViewModel(Option option)
        {
            if (option != null)
            {
                Id = option.Id;
                Title = option.Title;
                Description = option.Description;
                VotesCount = option.SurveyBulletins.Count;
                VotesCountString = DeclinationService.OfNumber(VotesCount, "голос", "голоса", "голосов");
                Position = option.Position;
            }
        }
    }
}