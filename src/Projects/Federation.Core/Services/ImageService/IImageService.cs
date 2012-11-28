using System.Drawing;

namespace Federation.Core
{
    public interface IImageService
    {
        string GetImageUrl<T>(string imageName, bool relativePath, string mode) where T : class;
        void DeleteImage<T>(string imageName) where T : class;
        string CropExistedImage<T>(string imageName, int x1, int y1, int x2, int y2) where T : class;
        Size GetImageSize<T>(string imageName) where T : class;
    }
}
