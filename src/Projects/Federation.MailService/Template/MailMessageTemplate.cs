using System;
using System.Text;

namespace Federation.MailService
{
    public abstract class MailMessageTemplate
    {
        private readonly StringBuilder buffer;

        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public DateTime Date { get; set; }

        protected MailMessageTemplate()
        {
            buffer = new StringBuilder();
        }

        public string Body
        {
            get { return buffer.ToString(); }
        }

        protected dynamic Model { get; private set; }

        public void SetModel(dynamic model)
        {
            UserId = model.UserId;
            UserFullName = model.UserFullName;
            UserEmail = model.UserEmail;
            Date = model.FeedDate;
            Model = model;
        }

        public abstract void Execute();

        public virtual void Write(object value)
        {
            WriteLiteral(value);
        }

        public virtual void WriteLiteral(object value)
        {
            buffer.Append(value);
        }
    }
}
