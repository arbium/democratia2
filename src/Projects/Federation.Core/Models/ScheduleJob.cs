using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Federation.Core
{
    public partial class ScheduleJob
    {
        public ScheduleJob()
        {
            Id = Guid.NewGuid();
        }
    }
}
