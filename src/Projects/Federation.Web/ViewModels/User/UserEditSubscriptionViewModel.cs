using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserEditSubscriptionViewModel
    {
        public IList<UserEditSubscription_GroupViewModel> Groups { get; set; }
        public IList<UserEditSubscription_UserViewModel> Users { get; set; }

        public UserEditSubscriptionViewModel()
        {
        }

        public UserEditSubscriptionViewModel(User user)
        {
            Groups = user.SubscriptionGroups.Select(x => new UserEditSubscription_GroupViewModel(x, user)).ToList();
            Users = user.SubscriptionUsers.Select(x => new UserEditSubscription_UserViewModel(x)).ToList();
        }
    }

    public class UserEditSubscription_GroupViewModel
    {
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool UserIsMember { get; set; }

        public UserEditSubscription_GroupViewModel()
        {
        }

        public UserEditSubscription_GroupViewModel(Group group, User user)
        {
            Logo = ImageService.GetImageUrl<Group>(group.Logo);
            Name = group.Name;
            Url = group.Url;

            var uig = GroupService.UserInGroup(user.Id, group.Id);
            UserIsMember = uig != null;
        }
    }

    public class UserEditSubscription_UserViewModel
    {
        public string Avatar { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public UserEditSubscription_UserViewModel()
        {
        }

        public UserEditSubscription_UserViewModel(User user)
        {
            Avatar = ImageService.GetImageUrl<User>(user.Avatar);
            Id = user.Id;
            Name = user.FullName;
        }
    }
}