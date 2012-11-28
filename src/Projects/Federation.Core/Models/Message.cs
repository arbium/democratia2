using System;

namespace Federation.Core
{
    public partial class Message
    {
        public Message()
        {
            Id = Guid.NewGuid();
        }
    }
}