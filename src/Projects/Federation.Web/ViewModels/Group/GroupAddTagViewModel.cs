using System.ComponentModel.DataAnnotations;
using Federation.Core;
using System;

namespace Federation.Web.ViewModels
{
    public class GroupAddTagViewModel
    {
        public string GroupName { get; set; }
        public string GroupUrl { get; set; }
        public Guid GroupId { get; set; }
        
        [Display(Name = "Заголовок темы группы", Description="Должно состоять из одного слова и только из букв или цифр")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Title { get; set; }

        [Display(Name = "Описание темы группы")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Description { get; set; }

        public GroupAddTagViewModel()
        {
        }

        public GroupAddTagViewModel(Group group)
        {
            if (group != null)
            {
                GroupName = group.Name;
                GroupUrl = group.Url;
                GroupId = group.Id;
            }
        }
    }
}