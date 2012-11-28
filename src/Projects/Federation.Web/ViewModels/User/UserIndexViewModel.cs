using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserIndexViewModel
    {
        public Guid UserId { get; set; }

        public bool ShowQuest { get; set; }        
        public QuestProgress QuestProgress { get; set; }

        public readonly User_SubscriptionViewModel Subscription = new User_SubscriptionViewModel();
        public readonly IList<UserMessageViewModel> VotingMessages = new List<UserMessageViewModel>();        

        public UserIndexViewModel()
        {
        }

        public UserIndexViewModel(User user)
        {
            if (user != null)
            {
                UserId = user.Id;

                var userQuestProgress = (QuestProgress)user.QuestProgress;
                if (!user.IsQuestRejected && !userQuestProgress.HasFlag(QuestProgress.Completed))
                    ShowQuest = true;

                QuestProgress = userQuestProgress;

                Subscription = new User_SubscriptionViewModel(user, DateTime.Now.AddMonths(-1), DateTime.Now);
                var expireDate = DateTime.Now.AddDays(-14);
                VotingMessages = user.InboxMessages
                    .Where(x => x.Date > expireDate)
                    .Where(x => !x.IsRead)
                    .Where(x => x.Type == (byte) MessageType.ElectionNotice || x.Type == (byte) MessageType.PetitionNotice || x.Type == (byte) MessageType.PollNotice)
                    .Take(5)
                    .Select(m => new UserMessageViewModel(m)).ToList();
            }
        }
    }
}