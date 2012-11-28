using System;

namespace Federation.Core
{
    public partial class GroupMember
    {
        public GroupMember()
        {
            Id = Guid.NewGuid();
        }
    }
}