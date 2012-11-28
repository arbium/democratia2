using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AccountSignUpViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Фамилия")]
        public string SurName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(ConstHelper.EmailRegexp, ErrorMessage = "Неправильный формат электронной почты")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Мобильный телефон")]
        [RegularExpression(ConstHelper.TelRegexp, ErrorMessage = "Неправильный формат номера телефона. Например: +9-999-9999999")]
        public string PhoneNumber { get; set; }

        public string Gender { get; set; }
        public bool ConnectSocial { get; set; }
        public SocialType SocialType { get; set; }
        public string ReturnUrl { get; set; }
    }
}