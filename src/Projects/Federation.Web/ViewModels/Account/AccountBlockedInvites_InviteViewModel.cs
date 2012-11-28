using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AccountBlockedInvites_InviteViewModel
    {
        public Guid InviteId { get; set; }
        public string FullName { get; set; }
        public string FacebookUrl { get; set; }
        public string LiveJournalUrl { get; set; }
        public string Comment { get; set; }

        public AccountBlockedInvites_InviteViewModel()
        {
        }

        public AccountBlockedInvites_InviteViewModel(Invite invite)
        {
            if (invite != null)
            {
                InviteId = invite.Id;
                FullName = invite.Surname + " " + invite.Name + " " + invite.Patronymic;
                FacebookUrl = invite.Facebook;
                LiveJournalUrl = invite.LiveJournal;
                Comment = invite.Comment;
            }
        }
    }
}