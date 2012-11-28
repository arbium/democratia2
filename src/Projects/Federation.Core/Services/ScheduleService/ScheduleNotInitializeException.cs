using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Federation.Core
{
    [Serializable]
    public class ScheduleNotInitializeException : ApplicationException
    {
        public ScheduleNotInitializeException() { }
        public ScheduleNotInitializeException(string message) : base(message) { }
        public ScheduleNotInitializeException(string message, Exception inner) : base(message, inner) { }
        protected ScheduleNotInitializeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
