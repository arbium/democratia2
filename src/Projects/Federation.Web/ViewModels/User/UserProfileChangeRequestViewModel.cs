using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserProfileChangeRequestViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Имя")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Display(Name = "Facebook")]
        public string Facebook { get; set; }

        [Display(Name = "LiveJournal")]
        public string LiveJournal { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Мобильный телефон")]
        [RegularExpression(ConstHelper.TelRegexp, ErrorMessage = "Неправильный формат номера телефона. Например: +7-999-9999999")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Причина изменения")]
        public string Comment { get; set; }

        public UserProfileChangeRequestViewModel()
        {            
        }

        public UserProfileChangeRequestViewModel(User user)
        {
            if (user != null)
            {
                UserId = user.Id;
                UserName = user.FullName;

                Firstname = user.FirstName;
                Surname = user.SurName;
                Patronymic = user.Patronymic;
                Facebook = user.Facebook;
                LiveJournal = user.LiveJournal;
                PhoneNumber = user.PhoneNumber;
            }
        }
    }
}