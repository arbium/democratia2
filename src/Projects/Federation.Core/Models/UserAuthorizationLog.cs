using System;

namespace Federation.Core
{
    public partial class UserAuthorizationLog
    {
        public UserAuthorizationLog()
        {
            Id = Guid.NewGuid();
        }
    }
}
