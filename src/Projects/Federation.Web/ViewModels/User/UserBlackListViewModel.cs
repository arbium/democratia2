using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserBlackListViewModel
    {
        public IList<UserBlackList_UserViewModel> BlackList = new List<UserBlackList_UserViewModel>();

        public UserBlackListViewModel()
        {            
        }

        public UserBlackListViewModel(User user)
        {
            if (user == null)
                return;

            BlackList = user.BlackList.Select(x => new UserBlackList_UserViewModel(x)).ToList();
        }
    }

    public class UserBlackList_UserViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }

        public UserBlackList_UserViewModel()
        {            
        }

        public UserBlackList_UserViewModel(User user)
        {
            if (user == null)
                return;

            Id = user.Id;
            FullName = user.FullName;
            Avatar = ImageService.GetImageUrl<User>(user.Avatar);
        }
    }
}