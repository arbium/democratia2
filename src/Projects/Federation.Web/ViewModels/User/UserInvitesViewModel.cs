using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserInvitesViewModel
    {
        public IList<UserInvites_InviteViewModel> Invites { get; set; }

        public UserInvitesViewModel()
        {
            Invites = new List<UserInvites_InviteViewModel>();
        }

        public UserInvitesViewModel(User user)
        {
            if (user != null)
                Invites = user.Invites
                    .OrderByDescending(x => x.CreationDate).Select(x => new UserInvites_InviteViewModel(x)).ToList();
            else
                Invites = new List<UserInvites_InviteViewModel>();
        }
    }

    public class UserInvites_InviteViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsResendEnabled { get; set; }
        public string UserUrl { get; set; }
        public string UserAvatar { get; set; }

        public UserInvites_InviteViewModel(Invite invite)
        {
            if (invite != null)
            {
                Id = invite.Id;
                FullName = invite.FullName;
                Email = invite.Email;
                State = invite.GetStateSting();
                CreationDate = invite.CreationDate;

                if(invite.State == (byte)InviteState.Used) {
                    UserUrl = UrlHelper.GetUrl<User>(invite.InvitedUser.Id);
                    var theUser = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == invite.InvitedUser.Id);
                    if (theUser != null)
                        UserAvatar = ImageService.GetImageUrl<User>(theUser.Avatar);
                }

                if (invite.State == (byte)InviteState.NotSent || invite.State == (byte)InviteState.Sent)
                    if (DateTime.Now - invite.CreationDate > ConstHelper.MinInviteResendInterval)
                        IsResendEnabled = true;
            }
        }
    }
}