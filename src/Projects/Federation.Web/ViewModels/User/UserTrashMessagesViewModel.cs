using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserTrashMessagesViewModel
    {
        public readonly IList<UserMessageViewModel> Messages = new List<UserMessageViewModel>();

        public UserTrashMessagesViewModel()
        {
        }

        public UserTrashMessagesViewModel(User user)
        {
            var inbox = user.InboxMessages.Where(x => x.IsDeletedByRecipient);
            var outbox = user.OutboxMessages.Where(x => x.IsDeletedByAuthor);
            var messages = inbox.Union(outbox).ToList();

            Messages = messages.Select(x => new UserMessageViewModel(x)).OrderByDescending(x => x.Date).ToList();
        }
    }
}