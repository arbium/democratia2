using System.ComponentModel.DataAnnotations;
namespace Federation.Web.ViewModels
{
    public class SignInViewModel
    {
        public string ReturnUrl { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Электропочта или логин")]
        public string UserIdentificator { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool IsPermanent { get; set; }

        public SignInViewModel()
        {
            IsPermanent = true;
        }
    }
}