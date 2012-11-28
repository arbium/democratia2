using System;

namespace Federation.Core
{
    public partial class MailRecord
    {
        public MailRecord()
        {
            Id = Guid.NewGuid();
        }
    }
}