using System;

namespace Feedback
{
    public class FeedbackServiceImpl : IFeedbackService
    {
        #region Implementation of IFeedbackService

        public void AddFeedback(string authorName, string message, FeedbackType type, string authorId, string email)
        {
            using (FeedbackModelContainer container = new FeedbackModelContainer())
            {
                Report report = new Report
                                    {
                                        AuthorName = authorName,
                                        Text = message,
                                        Type = type,
                                        AuthorId = authorId,
                                        Email = email
                                    };
                container.Reports.AddObject(report);
                container.SaveChanges();
            }
        }

        public void Comment(Guid reportId, string authorName, string message, string email, string authorId)
        {
            using (FeedbackModelContainer container = new FeedbackModelContainer())
            {
                Comment comment = new Comment
                                     {
                                         ReportId = reportId,
                                         AuthorName = authorName,
                                         Text = message,
                                         Email = email,
                                         AuthorId = authorId
                                     };
                container.Comments.AddObject(comment);
                container.SaveChanges();
            }
        }

        public bool IsReportAuthor(Guid reportId, string authorId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
