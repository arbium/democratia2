using Federation.Core;
using System.Linq;
using System;

namespace Federation.Web.ViewModels
{
    public class User_ContactsViewModel
    {
        public Guid UserId { get; set; }
        public string LiveJournal { get; set; }
        public string Facebook { get; set; }
        
        public User_ContactsViewModel()
        {
        }

        public User_ContactsViewModel(User user)
        {
            UserId = user.Id;
            LiveJournal = user.LiveJournal;
            if (LiveJournal != null)
            {
                if (!LiveJournal.Contains("http:"))
                    LiveJournal = "http://" + LiveJournal;
            }

            var facebookAccount = user.SocialAccounts.SingleOrDefault(x=>x.SocialType == (byte)SocialType.Facebook);
            if (facebookAccount != null)
            {
                Facebook = facebookAccount.DirectUrl;
            }
        }
    }
}