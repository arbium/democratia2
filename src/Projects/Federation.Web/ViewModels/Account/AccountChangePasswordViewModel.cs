using System.ComponentModel.DataAnnotations;

namespace Federation.Web.ViewModels
{
    public class AccountChangePasswordViewModel
    {
        [Display(Name = "Старый пароль")]
        [Required(ErrorMessage = "Обязательное поле")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Новый пароль")]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}