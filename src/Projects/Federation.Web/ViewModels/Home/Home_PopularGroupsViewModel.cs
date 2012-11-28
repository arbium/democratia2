using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Home_PopularViewModel
    {
        public IList<Home_Popular_GroupViewModel> Groups { get; set; }
        public IList<Home_Popular_UserViewModel> Users { get; set; }

		public Home_PopularViewModel()
        {
        }
		
        public Home_PopularViewModel(IList<Group> groups, IList<User> users)
        {
            if (groups != null)
            {
                Groups = new List<Home_Popular_GroupViewModel>();

                foreach (var group in groups)
                    Groups.Add(new Home_Popular_GroupViewModel(group));
            }

            if (users != null)
            {
                Users = new List<Home_Popular_UserViewModel>();

                foreach (var user in users)
                    Users.Add(new Home_Popular_UserViewModel(user));
            }
        }
    }

    public class Home_Popular_UserViewModel
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public int SubscribersCount { get; set; }
        public int PostsCount { get; set; }

        public Home_Popular_UserViewModel()
        {
        }

        public Home_Popular_UserViewModel(User user)
        {
            if (user != null)
            {
                Id = user.Id;
                DisplayName = user.FirstName + " " + user.SurName;
                SubscribersCount = user.Subscribers.Count;
                //DataService.PerThread.ContentSet.OfType<Post>().Where(x => x.AuthorId == user.Id && x.State == (byte)ContentState.Approved)
                PostsCount = DataService.PerThread.ContentSet.OfType<Post>().Where(x => x.AuthorId == user.Id && x.State == (byte)ContentState.Approved).Count();
            }
        }

    }

    public class Home_Popular_GroupViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public int MembersCount { get; set; }
        public int PostsCount { get; set; }

        public Home_Popular_GroupViewModel()
        {
        }

        public Home_Popular_GroupViewModel(Group group)
        {
            if (group != null)
            {
                Id = group.Id;
                Url = group.Url;
                Name = group.Name;
                MembersCount = DataService.PerThread.GroupMemberSet.Where(x => x.GroupId == group.Id && (x.State == (byte)GroupMemberState.Approved || x.State == (byte)GroupMemberState.Moderator)).Count();
                PostsCount = DataService.PerThread.ContentSet.OfType<Post>().Count(x => x.GroupId == group.Id && x.State == (byte)ContentState.Approved);
            }
        }
    }
}