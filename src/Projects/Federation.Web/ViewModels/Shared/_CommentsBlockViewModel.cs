using System;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _CommentsBlockViewModel
    {
        public Guid ContentId { get; set; }        
        public bool IsDiscussionClosed { get; set; }
        public bool IsDiscussionUnavailable { get; set; }

        public readonly _CommentsViewModel Comments = new _CommentsViewModel();

        public _CommentsBlockViewModel()
        {
        }

        public _CommentsBlockViewModel(Content content, bool? invert = null)
        {
            if (content != null)
            {
                ContentId = content.Id;
                IsDiscussionClosed = content.IsDiscussionClosed;

                if (content.GroupId.HasValue && UserContext.Current != null && !GroupService.IsUserApprovedInGroup(UserContext.Current.Id, content.Group) && content.Group.PrivacyEnum.HasFlag(GroupPrivacy.PrivateDiscussion))
                    IsDiscussionUnavailable = true;

                Comments = new _CommentsViewModel(content, invert);
            }
        }
    }
}