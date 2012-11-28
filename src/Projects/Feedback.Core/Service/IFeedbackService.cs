using System;

namespace Feedback
{
    public interface IFeedbackService
    {
        void AddFeedback(string authorName, string message, FeedbackType type, string email, string authorId);
        void Comment(Guid reportId, string authorName, string message, string email = null, string authorId = null);
        bool IsReportAuthor(Guid reportId, string authorId);
    }
}
