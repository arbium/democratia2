using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Federation.Core
{
    public partial class ScheduleExecutionInfo
    {
        public ScheduleExecutionInfo()
        {
            Id = Guid.NewGuid();
        }
    }
}
