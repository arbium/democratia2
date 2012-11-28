using System.Collections.Generic;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AccountBlockedInvitesViewModel
    {
        public IList<AccountBlockedInvites_InviteViewModel> Invites { get; set; }

        public AccountBlockedInvitesViewModel()
        {
            Invites = new List<AccountBlockedInvites_InviteViewModel>();
        }

        public AccountBlockedInvitesViewModel(IList<Invite> invites)
        {
            Invites = new List<AccountBlockedInvites_InviteViewModel>();
            if (invites != null)
            {
                foreach (var invite in invites)
                {
                    Invites.Add(new AccountBlockedInvites_InviteViewModel(invite));
                }
            }
        }

    }
}