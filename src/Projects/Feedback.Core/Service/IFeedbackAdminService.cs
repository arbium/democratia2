using System;

namespace Feedback
{
    interface IFeedbackAdminService
    {
        void Close(Guid reportId);
        void OfficiallyReply(Guid reportId, string reply);
    }
}
