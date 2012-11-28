using System;
using System.Linq;
using System.Net.Mail;

namespace Federation.Core
{
    public static class FeedbackService
    {
        public static void SendMessage(Guid userId, string subject, string text)
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Указан неверный идентификатор");

            var smtp = new SmtpClient();
            var message = new MailMessage
            {
                Subject = "Фидбэк: " + (string.IsNullOrWhiteSpace(subject) ? "..." : subject),
                Body = "Получен фидбек от <a href='" + UrlHelper.GetUrl<User>(user.Id, false) + "'>" + 
                    user.FullName + "</a><br />" + text,
                IsBodyHtml = true
            };

            message.To.Add("support@democratia2.ru");

            smtp.Send(message);
            smtp.Dispose();
        }

        public static void SendMessage(string name, string email, string subject, string text)
        {
            var smtp = new SmtpClient();
            var message = new MailMessage
            {
                Subject = "Фидбэк: " + (string.IsNullOrWhiteSpace(subject) ? "..." : subject),
                Body = "Получен фидбек от " + 
                    (string.IsNullOrWhiteSpace(email) ? string.Empty : "<a href='mailto:" + email + "'>") + 
                    (string.IsNullOrWhiteSpace(name) ? "Аноним" : name) + "</a><br />" + text,
                IsBodyHtml = true
            };

            message.To.Add("support@democratia2.ru");

            smtp.Send(message);
            smtp.Dispose();
        }
    }
}