using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Federation.Web.ViewModels
{
    public class _AddCommentViewModel
    {
        public Guid? ContentId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Текст")]
        [Required(ErrorMessage = "Обязательное поле")]
        [AllowHtml]
        public string Text { get; set; }
    }
}