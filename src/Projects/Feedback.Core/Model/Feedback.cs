using System;

namespace Feedback
{
    public partial class Report
    {
        public Report()
        {
            Id = Guid.NewGuid();
        }

        public FeedbackType Type
        {
            get { return (FeedbackType) TypeId; }
            set { TypeId = (byte)value; }
        }
    }
}
