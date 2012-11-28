using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserDialogViewModel
    {
        public Guid? ContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactAvatar { get; set; }

        [Display(Name = "Текст сообщения")]
        [Required(ErrorMessage = "Укажите текст сообщения")]
        public string Text { get; set; }

        public IList<UserDialog_MessageViewModel> Messages { get; set; }

        public UserDialogViewModel()
        {
        }

        public UserDialogViewModel(User contact)
        {
            if (contact == null)
                Messages = DataService.PerThread.MessageSet
                    .Where(x => !x.AuthorId.HasValue && x.RecipientId == UserContext.Current.Id && !x.IsDeletedByRecipient)
                    .OrderBy(x => x.Date).ToList()
                    .Select(x => new UserDialog_MessageViewModel(x)).ToList();
            else
            {
                ContactId = contact.Id;
                ContactName = contact.FirstName + " " + contact.SurName;
                ContactAvatar = ImageService.GetImageUrl<User>(contact.Avatar);
                Messages = DataService.PerThread.MessageSet
                    .Where(x => (x.AuthorId == contact.Id && x.RecipientId == UserContext.Current.Id && !x.IsDeletedByRecipient) || (x.RecipientId == contact.Id && x.AuthorId == UserContext.Current.Id && !x.IsDeletedByAuthor))
                    .OrderBy(x => x.Date)
                    .ToList().Select(x => new UserDialog_MessageViewModel(x)).ToList();
            }            
        }
    }

    public class UserDialog_MessageViewModel
    {
        public Guid Id { get; set; }
        public string[] Text { get; set; }
        public string Html { get; set; }
        public string Date { get; set; }
        public bool IsRead { get; set; }
        public bool IsInbox { get; set; }
        public MessageType Type { get; set; }

        public UserDialog_MessageViewModel()
        {
        }

        public UserDialog_MessageViewModel(Message message)
        {
            if (message == null)
                return;

            Id = message.Id;
            Text = message.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            Html = message.Text;
            Date = message.Date.ToFormattedUserTime(UserContext.Current);
            IsRead = message.IsRead;
            IsInbox = UserContext.Current.Id != message.AuthorId;
            Type = (MessageType)message.Type;
        }
    }
}