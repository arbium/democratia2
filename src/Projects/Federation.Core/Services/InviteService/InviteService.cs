using System;

namespace Federation.Core
{
    public static class InviteService
    {
        private static IInviteService _current = new InviteServiceImpl();

        public static Invite CreateInvite(InviteDataStruct inviteData)
        {
            return _current.CreateInvite(inviteData);
        }

        public static void SendInvite(Guid id)
        {
            _current.SendInvite(id);
        }

        public static void SendAllInvites()
        {
            _current.SendAllInvites();
        }

        public static Invite GetInvite(string key)
        {
            return _current.GetInvite(key);
        }

        public static Invite GiveInvite(Guid id)
        {
            return _current.GiveInvite(id);
        }
    }
}
