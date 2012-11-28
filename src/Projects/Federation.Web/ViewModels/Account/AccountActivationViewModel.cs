using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AccountActivationViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public Guid Id { get; set; }

        [Display(Name = "Email, к которому привязан аккаунт")]
        public string AccountMail { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(ConstHelper.EmailRegexp, ErrorMessage = "Неправильный формат электронной почты")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email для рассылок")]
        public string SubscribtionMail { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Мобильный телефон")]
        [RegularExpression(ConstHelper.TelRegexp, ErrorMessage = "Неправильный формат номера телефона. Например: +7-999-9999999")]
        public string Phone { get; set; }

        public bool ShowCode { get; set; }
        public Guid CodeId { get; set; }
        public string Code { get; set; }
        public bool JustRegistered { get; set; }
    }
}