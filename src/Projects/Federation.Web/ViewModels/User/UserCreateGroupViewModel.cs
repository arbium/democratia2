using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserCreateGroupViewModel
    {
        private string _cleanSummary;

        [Display(Name = "Название группы")]
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(80, ErrorMessage="Слишком длинное название группы")]
        public string Name { get; set; }

        [Display(Name = "Родительская группа")]
        [Required(ErrorMessage = "Обязательное поле")]
        public Guid ParentGroupId { get; set; }

        [Display(Name = "Человеко-понятный url")]
        [RegularExpression(@"^[A-Za-z0-9А-Яа-яёЁ/_\s]*$", ErrorMessage = "Человеко-понятный url должен содержать только буквы или цифры (пробелы будут заменены на символ нижнего подчеркивания )")]
        public string Label { get; set; }

        [Display(Name = "Описание")]
        [AllowHtml]
        public string Summary
        {
            get { return _cleanSummary; }
            set { _cleanSummary = TextHelper.CleanXssTags(HttpUtility.HtmlDecode(value)); }
        }

        public readonly IList<SelectListItem> CategoryList = new List<SelectListItem>();

        [Display(Name = "Категория")]
        public Guid? Category { get; set; }  // TODO: можно переделать на много категорий

        [Display(Name = "Количество модераторов")]
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(3, 10, ErrorMessage="Минимальное количество модераторов - 3, максимальное - 10")]
        public short ModeratorsCount { get; set; }

        [Display(Name = "Частота выборов (в месяцах)")]
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(1, 12, ErrorMessage = "Минимальная частота выборов - 1 месяц, максимальная - 12")]
        public short ElectionFrequency { get; set; }

        [Display(Name = "Кворум голосований (в процентах)")]
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(1, 100, ErrorMessage = "Минимальный кворум - 1, максимальный - 100")]
        public byte PollQuorum { get; set; }

        [Display(Name = "Кворум выборов (в процентах)")]
        [Required(ErrorMessage = "Обязательное поле")]
        [Range(1, 100, ErrorMessage = "Минимальный кворум - 1, максимальный - 100")]
        public byte ElectionQuorum { get; set; }

        [DataType(DataType.ImageUrl)]
        public string LogoUrl { get; set; }

        public UserCreateGroupViewModel()
        {
            ModeratorsCount = 3;
            ElectionFrequency = 6;
            ElectionQuorum = 30;
            PollQuorum = 20;

            CategoryList.Add(new SelectListItem());
            foreach (var gc in DataService.PerThread.GroupCategorySet)
                CategoryList.Add(new SelectListItem { Text = gc.Title, Value = gc.Id.ToString() });
        }
    }
}