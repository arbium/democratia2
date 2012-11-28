
namespace Feedback
{
    public interface IFeedbackSystemService
    {
        void AttachFeedback(Report report);
        void AttachComment(Comment comment);
        void DetachFeedback(Report report);
        void DetachComment(Comment comment);
        void DeleteFeedback(Report report);
        void DeleteComment(Comment comment);
        void SaveChanges();
    }
}
