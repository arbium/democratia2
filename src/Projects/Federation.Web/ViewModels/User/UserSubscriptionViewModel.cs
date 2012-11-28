using System;
using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserSubscriptionViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsEmailVerified { get; set; }
        public string SubscriptionEmail { get; set; }
        public DateTime? VerificationEmailSendDate { get; set; }

        [Display(Name = "Получать рассылку")]
        public bool IsSubscribed { get; set; }

        public UserSubscriptionViewModel()
        {            
        }

        public UserSubscriptionViewModel(User user)
        {
            UserId = user.Id;
            UserName = user.FullName;
            IsEmailVerified = user.IsEmailVerified;
            IsSubscribed = user.SubscriptionSettings.IsSubscribed;
            SubscriptionEmail = user.SubscriptionSettings.SubscriptionEmail;

            if (!IsEmailVerified)
                VerificationEmailSendDate = AccountService.IsEmailVerificationAlreadySend(user.Id);
        }
    }
}