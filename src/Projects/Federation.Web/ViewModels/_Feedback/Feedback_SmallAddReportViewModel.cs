using System.ComponentModel.DataAnnotations;
using Feedback;

namespace Federation.Web.ViewModels
{
    public class Feedback_SmallAddReportViewModel
    {
        [Display(Name = "Обратная связь")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Text { get; set; }

        [Display(Name = "Вид обратной связи")]
        [Required(ErrorMessage = "Обязательное поле")]
        public FeedbackType Type { get; set; }
    }
}