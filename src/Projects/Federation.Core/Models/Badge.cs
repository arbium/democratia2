using System;

namespace Federation.Core
{
    public partial class Badge
    {
        public Badge()
        {
            Id = Guid.NewGuid();
        }
    }
}