using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserDialogsViewModel
    {
        public readonly IList<UserDialogs_DialogViewModel> Dialogs = new List<UserDialogs_DialogViewModel>();

        public UserDialogsViewModel()
        {
            var userId = UserContext.Current.Id;

            var lastSysMessage = DataService.PerThread.MessageSet.Where(x => !x.AuthorId.HasValue && x.RecipientId == userId && !x.IsDeletedByRecipient)
                .OrderByDescending(x => x.Date).FirstOrDefault();

            if (lastSysMessage != null)
                Dialogs.Add(new UserDialogs_DialogViewModel(lastSysMessage));

            var lastPrivateMessages = DataService.PerThread.MessageSet
                .Where(x => x.Type == (byte)MessageType.PrivateMessage && (x.AuthorId == userId && !x.IsDeletedByAuthor || x.RecipientId == userId && !x.IsDeletedByRecipient))
                .GroupBy(x => x.AuthorId != userId ? x.AuthorId : x.RecipientId).Select(x => x.OrderByDescending(a => a.Date).FirstOrDefault()).ToList()
                .Select(x => new UserDialogs_DialogViewModel(x)).OrderByDescending(x => x.DateTime).ToList();

            Dialogs = Dialogs.Union(lastPrivateMessages).ToList();
        }
    }

    public class UserDialogs_DialogViewModel
    {
        public Guid? ContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactAvatar { get; set; }
        public DateTime DateTime { get; set; }
        public string MessageSummary { get; set; }
        public bool IsInbox { get; set; }
        public bool IsRead { get; set; }

        public UserDialogs_DialogViewModel()
        {
        }

        public UserDialogs_DialogViewModel(Message message)
        {
            if (message == null)
                return;

            if (message.AuthorId.HasValue)
            {
                var contact = UserContext.Current.Id != message.AuthorId ? (User)message.Author : (User)message.Recipient;
                ContactId = contact.Id;
                ContactName = contact.FirstName + " " + contact.SurName;
                ContactAvatar = ImageService.GetImageUrl<User>(contact.Avatar);
            }            

            var text = TextHelper.CleanTags(message.Text);
            MessageSummary = text.Length > ConstHelper.MessageSummaryLength ? text.Substring(0, ConstHelper.MessageSummaryLength) + "…" : text;
            
            DateTime = message.Date;
            IsInbox = UserContext.Current.Id != message.AuthorId;
            IsRead = message.IsRead;
        }
    }
}