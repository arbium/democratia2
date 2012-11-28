using System.ComponentModel.DataAnnotations;

namespace Federation.Web.ViewModels
{
    /// <summary>
    /// Форма добавления feedback-a. Используется для получения feedback-a от незарегестрированных пользователей системы. 
    /// </summary>
    public class Feedback_GuestAddReportViewModel : Feedback_AddReportViewModel
    {
        [Display(Name = "Имя советчика")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string AuthorName { get; set; }

        [Display(Name = "Электропочта советчика")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Email { get; set; }
    }
}