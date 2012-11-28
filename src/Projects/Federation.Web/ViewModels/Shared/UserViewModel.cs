using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserViewModel
    {
        public string Avatar { get; set; }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
		public string Patronymic { get; set; }

        public UserViewModel()
        {
        }

        public UserViewModel(User user)
        {
            if (user != null)
            {
                Avatar = ImageService.GetImageUrl<User>(user.Avatar);
                Id = user.Id;
                FirstName = user.FirstName;
                SurName = user.SurName;
				Patronymic = user.Patronymic;
            }
        }
    }
}