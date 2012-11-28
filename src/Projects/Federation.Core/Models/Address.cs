using System;

namespace Federation.Core
{
    public partial class Address
    {
        public Address()
        {
            Id = Guid.NewGuid();
        }
    }
}