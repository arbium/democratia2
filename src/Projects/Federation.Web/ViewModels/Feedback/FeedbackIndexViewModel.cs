using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Federation.Web.ViewModels
{
    public class FeedbackIndexViewModel
    {
        public Guid? UserId { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Тема")]
        public string Subject { get; set; }
        
        [Display(Name = "Текст")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Text { get; set; }

        public Guid SafetyKey { get; set; }
        public string SafetyImageUrl { get; set; }

        [DisplayName("Что изображено на картинке?")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Answer { get; set; }

        [DisplayName("Подсказка")]
        public string Tip { get; set; }
    }   
}