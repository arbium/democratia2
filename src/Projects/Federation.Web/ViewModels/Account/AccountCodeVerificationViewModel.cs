using System;
using System.ComponentModel.DataAnnotations;

namespace Federation.Web.ViewModels
{
    public class AccountCodeVerificationViewModel
    {
        public Guid UserId { get; set; }

        [Display(Name = "Секретный код")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string SecretCode { get; set; }
    }
}