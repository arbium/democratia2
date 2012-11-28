using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class InviteSignUpWithKeyViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Код приглашения")]
        public string InviteKey { get; set; }

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
        [Display(Name = "Номер телефона")]
        [DataType(DataType.PhoneNumber, ErrorMessage="Неправильный формат телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Часовой пояс")]
        public string UTCOffset { get; set; }

        public string ReturnUrl { get; set; }
    }
}