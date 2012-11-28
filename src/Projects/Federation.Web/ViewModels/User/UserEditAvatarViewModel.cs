using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserEditAvatarViewModel
    {
        [DataType(DataType.ImageUrl)]
        public string Url { get; set; }

        [Display(Name = "Файл с вашим фото")]
        public string AvatarUrl { get; set; }

        public string AvatarImageName { get; set; }

        public bool HasAvatar { get; set; }

        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public UserEditAvatarViewModel()
        {
        }

        public UserEditAvatarViewModel(User user)
        {
            AvatarImageName = user.Avatar;

            AvatarUrl = ImageService.GetImageUrl<User>(user.Avatar);

            if (AvatarUrl == ConstHelper.DefaultImageName)
                HasAvatar = false;
            else
            {
                HasAvatar = true;

                var size = ImageService.GetImageSize<User>(user.Avatar);
                ImageWidth = size.Width;
                ImageHeight = size.Height;
            }
        }
    }
}