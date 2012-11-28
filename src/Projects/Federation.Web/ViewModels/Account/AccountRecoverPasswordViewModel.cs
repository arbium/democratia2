using System.ComponentModel.DataAnnotations;

namespace Federation.Web.ViewModels
{
    public class AccountRecoverPasswordViewModel
    {        
        [Display(Name = "Электропочта")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Email { get; set; }
    }
}