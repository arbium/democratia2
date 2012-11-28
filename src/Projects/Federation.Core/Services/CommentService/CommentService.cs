using System;
using System.Linq;

namespace Federation.Core
{
    public static class CommentService
    {
        public static Comment AddComment(string text, Guid userId, Guid? contentId, Guid? parentCommentId)
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(u => u.Id == userId);
            if (user == null)
                throw new BusinessLogicException("Перезайдите");

            if (string.IsNullOrWhiteSpace(text))
                throw new BusinessLogicException("Не введен текст комментария");

            if (!contentId.HasValue && !parentCommentId.HasValue)
                throw new BusinessLogicException("Неверный идентификатор поста");

            var comment = new Comment
            {
                Text = text,
                UserId = userId
            };

            if (parentCommentId.HasValue)
            {
                var parentComment = DataService.PerThread.CommentSet.SingleOrDefault(c => c.Id == parentCommentId);
                if (parentComment == null)
                    throw new MvcActionRedirectException("Указан неверный идентификатор комментария", "error", "index");//TODO

                var group = parentComment.Content.Group;
                
                if (group.PrivacyEnum.HasFlag(GroupPrivacy.PrivateDiscussion) && !GroupService.IsUserApprovedInGroup(userId, group))
                    throw new BusinessLogicException("Комментировать могут только члены группы");

                if (parentComment.IsHidden)
                    throw new BusinessLogicException("Данный комментарий скрыт, его нельзя комментировать");

                if (parentComment.ParentComment != null)
                {
                    comment.ReplyTo = parentComment;
                    comment.ParentCommentId = parentComment.ParentCommentId;
                    comment.ContentId = parentComment.ParentComment.ContentId;
                }
                else
                {
                    comment.ParentCommentId = parentCommentId;
                    comment.ContentId = parentComment.ContentId;
                }
            }
            else
            {
                var content = DataService.PerThread.ContentSet.SingleOrDefault(c => c.Id == contentId);
                if (content == null)
                    throw new BusinessLogicException("Указан неверный идентификатор контента");

                if (content.IsDiscussionClosed)
                    throw new BusinessLogicException("Дискуссия по данному вопросу закрыта");

                comment.ContentId = contentId.Value;
            }

            DataService.PerThread.CommentSet.AddObject(comment);
            DataService.PerThread.SaveChanges();

            //сообщение о появлении коммента
            DataService.PerThread.LoadProperty(comment, c => c.Content);
            var commentAuthorId = Guid.Empty;

            if (comment.ParentComment != null)
            {
                if (comment.ReplyTo == null) //Отвечаем на корень ветки
                {
                    if (comment.ParentComment.UserId != comment.UserId)
                        commentAuthorId = comment.ParentComment.UserId;
                }
                else //Отвечаем на комментарий внутри ветки
                {

                    if (comment.ReplyTo.UserId != comment.UserId)
                        commentAuthorId = comment.ReplyTo.UserId;
                }
            }
            else //Отвечаем на пост
            {
                if (comment.Content.AuthorId != null)
                    if (comment.Content.AuthorId != comment.UserId)
                        commentAuthorId = comment.Content.AuthorId.Value;
            }

            if (commentAuthorId != Guid.Empty && comment.User.BlackListsOwners.Count(x => x.Id == commentAuthorId) == 0)
                MessageService.Send(new MessageStruct
                    {
                        RecipientId = commentAuthorId,
                        Text = MessageComposer.ComposeCommentNotice(comment.Id, comment.Text),
                        Type = (byte) MessageType.CommentNotice,
                        Date = DateTime.Now
                    });

            return comment;
        }

        public static void HideComment(Guid id, bool saveChanges)
        {
            var comment = DataService.PerThread.CommentSet.SingleOrDefault(c => c.Id == id);
            if (comment == null)
                throw new BusinessLogicException("Указан неверный идентификатор комментария");

            if (comment.Replies.Count > 0)
                throw new BusinessLogicException("Нельзя удалять комментарий, на который ответили!");

            comment.IsHidden = true;

            if (saveChanges)
                DataService.PerThread.SaveChanges();
        }

        public static void UnhideComment(Guid id, bool saveChanges)
        {
            var comment = DataService.PerThread.CommentSet.SingleOrDefault(c => c.Id == id);
            if (comment == null)
                throw new BusinessLogicException("Указан неверный идентификатор комментария");

            comment.IsHidden = false;

            if (saveChanges)
                DataService.PerThread.SaveChanges();
        }

        public static void DeleteAllComments(Guid contentId, bool saveChanges)
        {
            var content = DataService.PerThread.ContentSet.SingleOrDefault(c => c.Id == contentId);

            if (content == null)
                throw new BusinessLogicException("Указан неверный идентификатор контента");

            var comments = DataService.PerThread.CommentSet.Where(c => c.ContentId == contentId);
            content.Comments.Clear();

            foreach (var comment in comments)
            {
                comment.ChildComments.Clear();
                comment.ParentComment = null;
                comment.Replies.Clear();
                comment.ReplyTo = null;
                comment.User = null;
                DataService.PerThread.CommentSet.DeleteObject(comment);
            }

            if (saveChanges)
                DataService.PerThread.SaveChanges();
        }
    }
}