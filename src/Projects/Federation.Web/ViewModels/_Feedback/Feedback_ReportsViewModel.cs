using System.ComponentModel.DataAnnotations;
using Feedback;

namespace Federation.Web.ViewModels
{
    /// <summary>
    /// Форма добавления feedback-a. Используется для получения feedback-a от незарегестрированных пользователей системы. 
    /// </summary>
    public class Feedback_ReportsViewModel
    {
        [Display(Name = "Обратная связь")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Text { get; set; }

        [Display(Name = "Вид обратной связи")]
        [Required(ErrorMessage = "Обязательное поле")]
        public FeedbackType Type { get; set; }

        [Display(Name = "Имя советчика")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string AuthorName { get; set; }

        [Display(Name = "Электропочта советчика")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Email { get; set; }
    }
}