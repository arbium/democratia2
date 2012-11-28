using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class InviteRequestViewModel
    {
        private string _cleanUserInfo;

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(ConstHelper.EmailRegexp, ErrorMessage = "Неправильный формат электронной почты")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Ссылка на профиль Facebook")]
        public string FacebookUrl { get; set; }

        [Display(Name = "Ссылка на профиль LiveJournal")]
        public string LiveJournalUrl { get; set; }

        [Display(Name = "О себе")]
        [AllowHtml]
        public string UserInfo 
        {
            get
            {
                return _cleanUserInfo;
            }
            set
            {
                _cleanUserInfo = TextHelper.CleanXssTags(HttpUtility.HtmlDecode(value));
            }
        }
    }
}