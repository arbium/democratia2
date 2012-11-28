using System.ComponentModel.DataAnnotations;
using Feedback;

namespace Federation.Web.ViewModels
{
    /// <summary>
    /// Форма добавления feedback-a. Используется для получения feedback-a от пользователей-участников системы. 
    /// </summary>
    public class Feedback_AddReportViewModel
    {
        [Display(Name = "Обратная связь")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Text { get; set; }

        [Display(Name = "Вид обратной связи")]
        [Required(ErrorMessage = "Обязательное поле")]
        public FeedbackType Type { get; set; }
    }
}