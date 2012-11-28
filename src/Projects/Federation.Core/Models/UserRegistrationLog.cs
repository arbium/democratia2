using System;

namespace Federation.Core
{
    public partial class UserRegistrationLog
    {
        public UserRegistrationLog()
        {
            Id = Guid.NewGuid();
        }
    }
}
