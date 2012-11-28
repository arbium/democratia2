using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserQuestViewModel
    {
        public Guid UserId { get; set; }
        public QuestProgress QuestProgress { get; set; }

        public UserQuestViewModel()
        {
        }

        public UserQuestViewModel(User user)
        {
            if (user != null)
            {
                UserId = user.Id;
                QuestProgress = (QuestProgress)user.QuestProgress;
            }
        }
    }
}