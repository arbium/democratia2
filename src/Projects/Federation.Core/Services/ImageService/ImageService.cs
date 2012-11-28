using System.Drawing;

namespace Federation.Core
{
    public static class ImageService
    {
        private static readonly IImageService Current = new ImageServiceImpl();

        public static string GetImageUrl<T>(string imageName, bool relativePath = true, string mode = "crop") where T : class
        {
            return Current.GetImageUrl<T>(imageName, relativePath, mode);
        }

        public static void DeleteImage<T>(string imageName) where T : class
        {
            Current.DeleteImage<T>(imageName);
        }

        public static string CropExistedImage<T>(string imageName, int x1, int y1, int x2, int y2) where T : class
        {
            return Current.CropExistedImage<T>(imageName, x1, y1, x2, y2);
        }

        public static Size GetImageSize<T>(string imageName) where T : class
        {
            return Current.GetImageSize<T>(imageName);
        }
    }
}
