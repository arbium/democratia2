using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _CommentsViewModel
    {
        public Guid? ContentId { get; set; }
        public Guid? UserId { get; set; }

        public int CommentsCount { get; set; }
        public bool IsDiscussionClosed { get; set; }
        public bool IsFork { get; set; }
        public Guid? ReplyTo { get; set; }
        public bool? Invert { get; set; }

        public readonly IList<_Comments_CommentViewModel> TopComments = new List<_Comments_CommentViewModel>();
        public PaginationList<_Comments_CommentViewModel, Comment> Comments { get; set; }

        public _CommentsViewModel()
        {
        }

        public _CommentsViewModel(Content content, bool? invert = null)
        {
            if (content != null)
            {
                Invert = invert;
                ContentId = content.Id;
                IsFork = false;
                IsDiscussionClosed = content.IsDiscussionClosed;
                ContentId = content.Id;

                var comments = content.Comments.Where(c => !c.ParentCommentId.HasValue);
                if (invert.HasValue && invert.Value)
                    comments = comments.OrderBy(c => c.DateTime);
                else
                    comments = comments.OrderByDescending(c => c.DateTime);

                Comments = comments.ToPaginationList(ConstHelper.CommentsPerPage, x => new _Comments_CommentViewModel(x));
                CommentsCount = content.Comments.Count(c => !c.IsHidden);

                if (CommentsCount > 3)
                    TopComments = content.Comments.Where(x => !x.IsHidden && x.Rating > 0).OrderByDescending(x => x.Rating).ThenByDescending(x => x.DateTime).Take(3).ToList()
                        .Select(x => new _Comments_CommentViewModel(x)).ToList();
            }
        }

        public _CommentsViewModel(User user, bool? invert = null)
        {
            if (user != null)
            {
                Invert = invert;
                UserId = user.Id;

                var comments = user.Comments.Where(x => !x.IsHidden);
                if (invert.HasValue && invert.Value)
                    comments = comments.OrderBy(c => c.DateTime);
                else
                    comments = comments.OrderByDescending(c => c.DateTime);

                Comments = comments.ToPaginationList(ConstHelper.CommentsPerPage, x => new _Comments_CommentViewModel(x));
            }
        }

        public _CommentsViewModel(IList<Comment> comments, Guid? replyTo = null, bool? invert = null)
        {
            Invert = invert;
            IsFork = true;
            ReplyTo = replyTo;

            var firstComment = comments.FirstOrDefault();
            if (firstComment != null)
            {
                IsDiscussionClosed = firstComment.Content.IsDiscussionClosed;
                ContentId = firstComment.Content.Id;
            }

            if (invert.HasValue && invert.Value)
                Comments = comments.OrderBy(c => c.DateTime).ToPaginationList(ConstHelper.CommentsPerPage, x => new _Comments_CommentViewModel(x));
            else
                Comments = comments.OrderByDescending(c => c.DateTime).ToPaginationList(ConstHelper.CommentsPerPage, x => new _Comments_CommentViewModel(x));

            CommentsCount = Comments.Count(c => !c.IsHidden);
        }
    }

    public class _Comments_CommentViewModel
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public Guid ContentId { get; set; }
        public string ContentUrl { get; set; }
        public string ContentTitle { get; set; }
        public string[] Text { get; set; }
        public bool IsHidden { get; set; }
        public int Rating { get; set; }

        public _Comments_CommentReplyViewModel ReplyComment { get; set; }
        public bool IsReplied { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserAvatar { get; set; }
        public string UserGeo { get; set; }

        public IList<_Comments_CommentViewModel> ChildComments { get; set; }
        public int VisibleChildComments { get; set; }

        public _Comments_CommentViewModel()
        {
        }

        public _Comments_CommentViewModel(Comment comment)
        {
            if (comment != null)
            {
                Id = comment.Id;
                ContentId = comment.ContentId;
                ContentUrl = comment.Content.GetUrl(comment);
                ContentTitle = comment.Content.Title;
                Text = comment.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
                IsHidden = comment.IsHidden;
                Rating = comment.Rating;
                DateTime = comment.DateTime;
                UserId = comment.UserId;
                UserName = comment.User.FirstName;
                UserSurname = comment.User.SurName;
                UserAvatar = ImageService.GetImageUrl<User>(comment.User.Avatar);
                if (comment.User.AddressId.HasValue)
                    UserGeo = comment.User.Address.City.Title;

                ChildComments = new List<_Comments_CommentViewModel>();
                if (comment.ParentCommentId == null)
                    ChildComments = comment.ChildComments
                        .OrderBy(c => c.DateTime).Select(c => new _Comments_CommentViewModel(c)).OrderBy(x => x.DateTime).ToList();

                VisibleChildComments = ChildComments.Count(x => !x.IsHidden);

                if (comment.ReplyTo != null)
                    ReplyComment = new _Comments_CommentReplyViewModel(comment.ReplyTo);

                IsReplied = comment.Replies.Count > 0;
            }
        }
    }

    public class _Comments_CommentReplyViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }

        public _Comments_CommentReplyViewModel()
        {
        }

        public _Comments_CommentReplyViewModel(Comment comment)
        {
            if (comment != null)
            {
                Id = comment.Id;
                UserId = comment.UserId;
                UserName = comment.User.FirstName;
                UserSurname = comment.User.SurName;
            }
        }
    }
}