using System;
using System.ComponentModel.DataAnnotations;

namespace Federation.Web.ViewModels
{
    public class AccountSetNewPasswordViewModel
    {
        public Guid RecoverKey { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }
    }
}