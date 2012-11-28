using System;

namespace Federation.Core
{
    public partial class BaseUser
    {
        public BaseUser()
        {
            Id = Guid.NewGuid();
        }
    }
}