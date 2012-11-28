using System.Collections.Generic;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class AccountRequestedInvitesViewModel
    {
        public IList<AccountRequestedInvites_InviteViewModel> Invites { get; set; }

        public AccountRequestedInvitesViewModel()
        {
            Invites = new List<AccountRequestedInvites_InviteViewModel>();
        }

        public AccountRequestedInvitesViewModel(IList<Invite> invites)
        {
            Invites = new List<AccountRequestedInvites_InviteViewModel>();
            if (invites != null)
            {
                foreach (var invite in invites)
                {
                    Invites.Add(new AccountRequestedInvites_InviteViewModel(invite));
                }
            }
        }

    }
}