using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserPayPalValidationViewModel
    {
        [Display(Name = "Электропочта (как на PayPal)")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Email { get; set; }

        [Display(Name = "Имя (как на PayPal)")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия (как на PayPal)")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string LastName { get; set; }
    }
}