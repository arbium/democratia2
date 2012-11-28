using System;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class _NavigationBoxViewModel
    {
        public Guid Id { get; set; }
        public NavigationBoxType Type { get; set; }
        public _NavigationBox_UserViewModel User;
        public _NavigationBox_GroupViewModel Group;

        public _NavigationBoxViewModel()
        {
        }

        public _NavigationBoxViewModel(Guid id, NavigationBoxType type)
        {
            Id = id;
            Type = type;
            switch(type)
            {
                case NavigationBoxType.Group:
                    {
                        var group = DataService.PerThread.GroupSet.SingleOrDefault(x => x.Id == id);
                        if (group != null)
                            Group = new _NavigationBox_GroupViewModel(group);
                    }
                    break;
                case NavigationBoxType.User:
                    {
                        var user = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == id);
                        if (user != null)
                            User = new _NavigationBox_UserViewModel(user);
                    }
                    break;
            }
        }

        public _NavigationBoxViewModel(User user)
        {
            Id = user.Id;
            Type = NavigationBoxType.User;
            User = new _NavigationBox_UserViewModel(user);
        }

        public _NavigationBoxViewModel(Group group)
        {
            Id = group.Id;
            Type = NavigationBoxType.Group;
            Group = new _NavigationBox_GroupViewModel(group);
        }
    }

    public enum NavigationBoxType
    {
        User,
        Group
    }

    //public class _NavigationBox_ContentViewModel
    //{
    //    public Guid Id { get; set; }
    //    public string Title { get; set; }
    //    public Guid? AuthorId { get; set; }
    //    public ContentState State { get; set; }
    //    public ContentViewType ContentType { get; set; }
    //    public bool IsAuthor { get; set; }
    //    public bool IsAttached { get; set; }
    //    public bool IsDiscussionClosed { get; set; }

    //    public _NavigationBox_ContentViewModel(Content content = null)
    //    {
    //        if (content != null)
    //        {
    //            Id = content.Id;
    //            Title = content.Title;
    //            AuthorId = content.AuthorId;
    //            State = (ContentState)content.State;

    //            IsDiscussionClosed = content.IsDiscussionClosed;
    //            if (content.GroupId.HasValue)
    //                IsAttached = content.IsGroupAttached(content.GroupId.Value);
    //            else if (content.AuthorId.HasValue)
    //                IsAttached = content.IsUserAttached(content.AuthorId.Value);

    //            if (UserContext.Current != null)
    //                IsAuthor = UserContext.Current.Id == AuthorId;

    //            if (content is Post)
    //            {
    //                if (content.GroupId.HasValue)
    //                    ContentType = ContentViewType.GroupPost;
    //                else if (content.AuthorId.HasValue)
    //                    ContentType = ContentViewType.UserPost;                    
    //            }
    //            else if (content is Petition)
    //            {
    //                if (content.GroupId.HasValue)
    //                    ContentType = ContentViewType.GroupPetition;
    //                else if (content.AuthorId.HasValue)
    //                    ContentType = ContentViewType.UserPetition;
    //            }
    //            else if (content is Poll)
    //            {
    //                ContentType = ContentViewType.Poll;
    //            }
    //            else if (content is Election)
    //            {
    //                ContentType = ContentViewType.Election;
    //            }
    //            else if (content is Survey)
    //            {
    //                ContentType = ContentViewType.Survey;
    //            }
    //        }
    //    }
    //}

    public class _NavigationBox_UserViewModel
    {
        public Guid Id { get; set; }
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Patronymic { get; set; }
        public string FullName { get; set; }
        public bool IsSubscriber { get; set; }
        public bool Self { get; set; }

        public _NavigationBox_UserViewModel(User user = null)
        {
            if (user != null)
            {
                Avatar = ImageService.GetImageUrl<User>(user.Avatar);
                Id = user.Id;
                FirstName = user.FirstName;
                SurName = user.SurName;
                Patronymic = user.Patronymic;
                FullName = user.FullName;

                if (UserContext.Current != null)
                {
                    var currUser = DataService.PerThread.BaseUserSet.OfType<User>().SingleOrDefault(x => x.Id == UserContext.Current.Id);

                    Self = UserContext.Current.Id == Id;

                    if (!Self && currUser != null)
                        IsSubscriber = currUser.SubscriptionUsers.Contains(user);
                }
                else
                    Self = false;
            }
        }
    }

    public class _NavigationBox_GroupViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }

        public bool IsMember { get; set; }
        public bool IsApproved { get; set; }
        public bool IsModerator { get; set; }
        public bool IsContentModerated { get; set; }
        public bool IsMembersModerated { get; set; }

        public bool HasChild { get; set; }

        public _NavigationBox_GroupViewModel(Group group = null)
        {
            if (group != null)
            {
                Logo = ImageService.GetImageUrl<Group>(group.Logo);
                Id = group.Id;
                Url = group.Url;
                Name = group.Name;
                IsContentModerated = group.PrivacyEnum.HasFlag(GroupPrivacy.ContentModeration);
                IsMembersModerated = group.PrivacyEnum.HasFlag(GroupPrivacy.MemberModeration);

                var firstChildGroup = DataService.PerThread.GroupSet.FirstOrDefault(x => x.ParentGroupId == Id);
                HasChild = firstChildGroup != null;

                if (UserContext.Current != null)
                {
                    IsMember = UserContext.Current.IsUserInGroup(Id);
                    IsApproved = UserContext.Current.IsUserApprovedInGroup(Id);
                    IsModerator = UserContext.Current.IsUserModeratorInGroup(Id);
                }
                else
                {
                    IsMember = false;
                    IsApproved = false;
                    IsModerator = false;
                }
            }
        }
    }
}