using System;

namespace Federation.Core
{
    [Flags]
    public enum GroupMessageRecipientType : byte
    {
        Members = 1,
        Moderators = 2,
        NotApprovedUsers = 4
    }
}
