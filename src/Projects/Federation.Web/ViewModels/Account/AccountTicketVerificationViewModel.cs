using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AccountTicketVerificationViewModel
    {
        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Display(Name = "Дата рождения")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Номер телефона")]
        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(ConstHelper.TelRegexp, ErrorMessage = "Неправильный формат номера телефона. Например: +7-999-9999999")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Идентификатор")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string TicketCode { get; set; }

        [Display(Name = "Одноразовый пароль")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string TempPass { get; set; }

        public bool IsTicketVerified { get; set; }

        public AccountTicketVerificationViewModel()
        {
        }

        public AccountTicketVerificationViewModel(User user)
        {
            if (user == null)
                return;

            if (!string.IsNullOrEmpty(user.PhoneNumber))
                PhoneNumber = user.PhoneNumber;

            FullName = user.FullName;
            BirthDate = user.BirthDate;
            IsTicketVerified = user.IsTicketVerified;
        }
    }
}