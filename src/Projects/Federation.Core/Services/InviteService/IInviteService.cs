using System;

namespace Federation.Core
{
    public interface IInviteService
    {
        Invite CreateInvite(InviteDataStruct inviteData);
        void SendInvite(Guid id);
        void SendAllInvites();
        Invite GetInvite(string key);
        Invite GiveInvite(Guid id);
    }
}
