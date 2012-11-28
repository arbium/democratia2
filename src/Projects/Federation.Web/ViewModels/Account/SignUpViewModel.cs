using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class SignUpViewModel
    {
        [Display(Name = "Логин")]
        public string Login { get; set; }

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

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}