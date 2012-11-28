
using System;
using System.Linq;

namespace Feedback
{
    public static class FeedbackService
    {
        private static readonly IFeedbackService _feedbackService = new FeedbackServiceImpl();
        private static readonly IFeedbackAdminService _feedbackAdminService = new FeedbackAdminServiceImpl();
        private static readonly IFeedbackSystemService _feedbackSystemService = new FeedbackSystemServiceImpl();

        #region Common

        public static void AddFeedback(string authorName, string message, FeedbackType type, string email = null, string authorId = null)
        {
            _feedbackService.AddFeedback(authorName, message, type, email, authorId);
        }

        public static bool IsReportAuthor(Guid reportId, string authorId)
        {
            return _feedbackService.IsReportAuthor(reportId, authorId);
        }

        public static void Comment(Guid reportId, string authorName, string message, string email = null, string authorId = null)
        {
            _feedbackService.Comment(reportId, authorName, message, email, authorId);
        }

        public static IQueryable<Report> Reports
        {
            get { return null; }
        }
        public static IQueryable<Comment> Comments
        {
            get { return null; }
        }

        #endregion

        public static class System
        {
            public static void AttachFeedback(Report report)
            {
                _feedbackSystemService.AttachFeedback(report);
            }

            public static void AttachComment(Comment comment)
            {
                _feedbackSystemService.AttachComment(comment);
            }

            public static void DetachFeedback(Report report)
            {
                _feedbackSystemService.DetachFeedback(report);
            }

            public static void DetachComment(Comment comment)
            {
                _feedbackSystemService.AttachComment(comment);
            }

            public static void DeleteFeedback(Report report)
            {
                _feedbackSystemService.DeleteFeedback(report);
            }

            public static void DeleteComment(Comment comment)
            {
                _feedbackSystemService.DetachComment(comment);
            }

            public static void SaveChanges()
            {
                _feedbackSystemService.SaveChanges();
            }
        }

        public static class Admin
        {
            public static void Close(Guid reportId)
            {
                _feedbackAdminService.Close(reportId);
            }

            public static void OfficiallyReply(Guid reportId, string reply)
            {
                _feedbackAdminService.OfficiallyReply(reportId, reply);
            }
        }
    }
}
