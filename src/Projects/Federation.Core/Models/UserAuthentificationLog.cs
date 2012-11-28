using System;

namespace Federation.Core
{
    public partial class UserAuthentificationLog
    {
        public UserAuthentificationLog()
        {
            Id = Guid.NewGuid();
        }
    }
}
