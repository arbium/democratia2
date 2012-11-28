using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupCreateEditSurveyViewModel
    {
        private string _cleanText;

        public Guid SurveyId { get; set; }

        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }
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

        [Display(Name = "Только для членов группы")]
        public bool IsPrivate { get; set; }

        [Display(Name = "Черновик")]
        public bool IsDraft { get; set; }

        [Display(Name = "Открытый опрос")]
        public bool HasOpenProtocol { get; set; }

        [Display(Name = "Тэги (через запятую)")]
        public string TagTitles { get; set; }

        [Display(Name = "Длительность, дней")]
        [Range(1, 32767, ErrorMessage = "Минимальное количество - 1, максимальное - 32767")]
        public short? Duration { get; set; }

        [Display(Name = "Вариантов можно выбрать, штук")]
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(1, 255, ErrorMessage = "Минимальное количество - 1, максимальное - 255")]
        public byte VariantsCount { get; set; }

        [Display(Name = "Варианты ответа")]
        [Required(ErrorMessage = "Обязательное поле")]
        public IList<GroupSurveyOptionViewModel> Options { get; set; }

        public GroupCreateEditSurveyViewModel()
        {
            VariantsCount = 1;

            Options = new List<GroupSurveyOptionViewModel>();
            Options.Add(new GroupSurveyOptionViewModel());
            Options.Add(new GroupSurveyOptionViewModel());
        }

        public GroupCreateEditSurveyViewModel(Group group)
        {
            VariantsCount = 1;

            Options = new List<GroupSurveyOptionViewModel>();
            Options.Add(new GroupSurveyOptionViewModel());
            Options.Add(new GroupSurveyOptionViewModel());

            if (group != null)
            {
                GroupId = group.Id;
                GroupName = group.Name;
                GroupUrl = group.Url;
                IsContentModeration = group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);

                if (GroupId == new Guid(ConstHelper.KsGroupId))
                {
                    HasOpenProtocol = true;
                }
            }
        }

        public GroupCreateEditSurveyViewModel(Survey survey)
        {
            if (survey != null)
            {
                if (survey.GroupId.HasValue)
                {
                    GroupId = survey.GroupId.Value;
                    GroupName = survey.Group.Name;
                    GroupUrl = survey.Group.Url;
                    IsContentModeration = survey.Group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);
                }

                SurveyId = survey.Id;
                Title = survey.Title;
                Text = survey.Text;
                IsPrivate = survey.IsPrivate;
                IsDraft = survey.State == (byte)ContentState.Draft;
                TagTitles = TagsHelper.ConvertTagListToString(survey.Tags);
                Duration = survey.Duration;
                VariantsCount = survey.VariantsCount;
                Options = survey.Options.Select(x => new GroupSurveyOptionViewModel(x)).OrderBy(x => x.Position).ToList();
            }
        }
    }
}