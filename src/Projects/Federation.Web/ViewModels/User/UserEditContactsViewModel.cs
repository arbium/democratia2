using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserEditContactsViewModel
    {
        [Display(Name = "Facebook")]
        [RegularExpression(ConstHelper.FbRegexp, ErrorMessage = "Неправильный формат ссылки")]
        public string Facebook { get; set; }

        [Display(Name = "LiveJournal")]
        [RegularExpression(ConstHelper.LjRegexp, ErrorMessage = "Неправильный формат ссылки")]
        public string LiveJournal { get; set; }

        [Display(Name = "Забирать записи из моего ЖЖ?")]
        public bool LJSindication { get; set; }

        [Display(Name = "Сохранять новые записи из ЖЖ как черновики?")]
        public bool LJPublishState { get; set; }

        public bool IsFbEditable { get; set; }
        public bool IsLjEditable { get; set; }

        public UserEditContactsViewModel()
        {
        }

        public UserEditContactsViewModel(User user)
        {
            Facebook = user.Facebook;
            LiveJournal = user.LiveJournal;
            LJPublishState = user.LiveJournalSindicateAsDraft;
            LJSindication = user.LiveJournalSindication;

            if (string.IsNullOrWhiteSpace(Facebook))
                IsFbEditable = true;
            if (string.IsNullOrWhiteSpace(LiveJournal))
                IsLjEditable = true;
        }
    }
}