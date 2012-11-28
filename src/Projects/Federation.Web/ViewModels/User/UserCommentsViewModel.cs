using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserCommentsViewModel
    {
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public readonly _CommentsViewModel Comments = new _CommentsViewModel();

        public UserCommentsViewModel(User user, bool? invert = null)
        {
            if (user != null)
            {
                UserId = user.Id;
                UserFullName = user.FullName;
                Comments = new _CommentsViewModel(user, invert);
            }
        }
    }
}
