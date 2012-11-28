using System;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserMessageViewModel
    {
        public Guid Id { get; set; }

        public Guid? AuthorId { get; set; }
        public string AuthorFullName { get; set; }

        public Guid RecipientId { get; set; }
        public string RecipientFullName { get; set; }

        public string[] Text { get; set; }
        public string Html { get; set; }
        public string Summary { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public byte Type { get; set; }

        public bool IsInbox { get; set; }

        public UserMessageViewModel()
        {
        }

        public UserMessageViewModel(Message message)
        {
            if (message == null)
                return;

            Id = message.Id;
            IsInbox = UserContext.Current.Id != message.AuthorId;

            if (message.AuthorId.HasValue)
            {
                AuthorId = message.AuthorId;
                AuthorFullName = ((User)message.Author).FullName;
            }

            RecipientId = message.RecipientId;
            RecipientFullName = ((User)message.Recipient).FullName;

            Text = message.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            Summary = TextHelper.CleanTags(message.Text);
            if (Summary.Length > ConstHelper.MessageSummaryLength)
                Summary = Summary.Substring(0, ConstHelper.MessageSummaryLength) + "…";

            Html = message.Text;
            Date = message.Date;
            IsRead = message.IsRead;
            Type = message.Type;
        }
    }
}