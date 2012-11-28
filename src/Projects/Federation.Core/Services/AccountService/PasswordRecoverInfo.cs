using System;

namespace Federation.Core
{
    public class PasswordRecoverInfo
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid RecoveryKey { get; set; }
    }
}
