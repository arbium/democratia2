using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserInviteUserViewModel
    {
        public Guid ReferalId { get; set; }
        public int GivenInvitesCount { get; set; }

        [Display(Name = "Электропочта")]
        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(ConstHelper.EmailRegexp, ErrorMessage="Неверный формат электропочты")]
        public string Email { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Name { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string SurName { get; set; }

        [Display(Name = "Отчество")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Patronymic { get; set; }

        [Display(Name = "О пользователе")]
        public string Info { get; set; }
    }
}