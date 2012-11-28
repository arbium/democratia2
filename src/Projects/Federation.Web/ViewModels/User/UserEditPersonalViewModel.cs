using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserEditPersonalViewModel
    {
        [Display(Name = "Пол")]
        public bool Sex { get; set; }

        [Display(Name = "Фамилия")]
        public string SurName { get; set; }

        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Отчество")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Patronymic { get; set; }

        [Display(Name = "Город")]
        public string City { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date, ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string BirthDate { get; set; }

        [Display(Name = "Место рождения")]
        public string BirthCity { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Часовой пояс")]
        public string UTCOffset { get; set; }

        public UserEditPersonalViewModel()
        {
        }

        public UserEditPersonalViewModel(User user)
        {
            SurName = user.SurName;
            FirstName = user.FirstName;
            Patronymic = user.Patronymic;
            UTCOffset = new TimeSpan(0, user.UTCOffset, 0).ToString();

            if (user.BirthDate.HasValue)
                BirthDate = user.BirthDate.Value.ToShortDateString();

            Sex = !user.Sex.HasValue || user.Sex.Value;

            if (user.AddressId != null)
                City = user.Address.City.Title;
        
            if (user.BirthAddressId != null)
                BirthCity = user.BirthAddress.City.Title;
        }
    }
}