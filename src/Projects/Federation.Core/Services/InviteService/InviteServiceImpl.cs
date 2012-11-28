using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;

namespace Federation.Core
{
    public class InviteServiceImpl: IInviteService
    {
        public Invite CreateInvite(InviteDataStruct inviteData)
        {
            if (string.IsNullOrWhiteSpace(inviteData.Name) || string.IsNullOrWhiteSpace(inviteData.Surname) || string.IsNullOrWhiteSpace(inviteData.Patronymic))
                throw new BusinessLogicException("Не заполнены ФИО");

            string key = null;
            if(inviteData.State != InviteState.Requested)
                key = Guid.NewGuid().ToString();

            var invite = new Invite()
            {
                Email = inviteData.Email,
                Facebook = inviteData.FacebookId,
                Key = key,
                LiveJournal = inviteData.LiveJournalId,
                Name = inviteData.Name,
                Surname = inviteData.Surname,
                Patronymic = inviteData.Patronymic,
                State = (byte)(inviteData.State ?? InviteState.NotSent),
                UserInfo = inviteData.UserInfo,
                CreationDate = DateTime.Now,
                Phone = inviteData.Phone
            };

            DataService.PerThread.InviteSet.AddObject(invite);
            DataService.PerThread.SaveChanges();

            return invite;
        }

        public void SendInvite(Guid id)
        {
            Invite invite = DataService.PerThread.InviteSet.SingleOrDefault(i => i.Id == id);

            if (invite == null)
                throw new BusinessLogicException("Не найден инвайт");

            _SendInvite(invite);
        }

        private void _SendInvite(Invite invite)
        {
            if (!string.IsNullOrWhiteSpace(invite.Email))
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress("noreply@democratia2.ru");
                message.To.Add(new MailAddress(invite.Email));
                message.Subject = "Приглашение на Демократия2";

                if (invite.Referal != null)
                    message.Body = MessageComposer.ComposeInvitationMessage(invite.Referal.Id, invite.Name + " " + invite.Patronymic, invite.Key);
                else
                    message.Body = MessageComposer.ComposeInvitationMessage(invite.Name + " " + invite.Patronymic, invite.Key);

                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Send(message);

                smtp.Dispose();
                message.Dispose();

                invite.CreationDate = DateTime.Now;
                invite.State = (byte)InviteState.Sent;

                DataService.PerThread.SaveChanges();
            }
        }

        public void SendAllInvites()
        {
            IList<Invite> invites = DataService.PerThread.InviteSet.Where(i => i.State == (byte)InviteState.NotSent).ToList();

            foreach (Invite invite in invites)
            {
                _SendInvite(invite);
                Thread.Sleep(500);
            }
        }

        public Invite GetInvite(string key)
        {
            var invite = DataService.PerThread.InviteSet.SingleOrDefault(i => i.Key == key);
            if (invite != null)
                return invite;

            return null;
        }

        public Invite GiveInvite(Guid id)
        {
            var invite = DataService.PerThread.InviteSet.SingleOrDefault(i => i.Id == id && i.State == (byte)InviteState.Requested);

            if (invite != null)
            {
                invite.State = (byte)InviteState.NotSent;
                invite.Key = Guid.NewGuid().ToString();
                DataService.PerThread.SaveChanges();
                _SendInvite(invite);
            }

            return invite;
        }
    }
}
