using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class ReplyCommentViewModel
    {
        public _AddCommentViewModel AddComment { get; set; }
        public _Comments_CommentViewModel Comment { get; set; }

        public string ContentTitle { get; set; }

        public Guid? AuthorId { get; set; }
        public Guid? GroupId { get; set; }

        public ReplyCommentViewModel()
        {
            AddComment = new _AddCommentViewModel();
            Comment = new _Comments_CommentViewModel();
        }

        public ReplyCommentViewModel(Comment parentComment)
        {
            if (parentComment != null)
            {
                ContentTitle = parentComment.Content.Title;
                
                AuthorId = parentComment.Content.AuthorId;
                GroupId = parentComment.Content.GroupId;

                AddComment = new _AddCommentViewModel { ParentCommentId = parentComment.Id, ReturnUrl = parentComment.Content.GetUrl() };

                Comment = new _Comments_CommentViewModel(parentComment);
            }
        }
    }
}