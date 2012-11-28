using System.ComponentModel.DataAnnotations;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupEditLogoViewModel
    {
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Url { get; set; }

        [Display(Name = "Файл с вашим логотипом")]
        public string LogoUrl { get; set; }

        public string LogoImageName { get; set; }

        public bool HasLogo { get; set; }

        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public GroupEditLogoViewModel()
        {
        }

        public GroupEditLogoViewModel(Group group)
        {
            GroupUrl = group.Url;
            GroupName = group.Name;

            LogoImageName = group.Logo;

            LogoUrl = ImageService.GetImageUrl<Group>(group.Logo);

            if (LogoUrl == ConstHelper.DefaultImageName)
                HasLogo = false;
            else
            {
                HasLogo = true;
                var size = ImageService.GetImageSize<Group>(group.Logo);

                ImageWidth = size.Width;
                ImageHeight = size.Height;
            }
        }
    }
}