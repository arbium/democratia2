using System;

namespace Federation.Core
{
    public partial class BlockedRecord
    {
        public BlockedRecord()
        {
            Id = Guid.NewGuid();
        }
    }
}