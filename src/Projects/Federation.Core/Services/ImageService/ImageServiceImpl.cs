using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;

namespace Federation.Core
{
    public class ImageServiceImpl : IImageService
    {
        private static readonly HashSet<string> ActiveWork = new HashSet<string>();
        
        /// <typeparam name="T">User/Group/Badge/AlbumItem</typeparam>
        public string GetImageUrl<T>(string imageName, bool relativePath, string mode) where T : class
        {
            var applicationPath = relativePath ? "/MediaContent/" : ConstHelper.AppUrl + "/MediaContent/";

            if (typeof (T) == typeof (User))
                applicationPath += "Avatars/";
            else if (typeof (T) == typeof (Group))
                applicationPath += "GroupLogos/";
            else if (typeof (T) == typeof (Badge))
                applicationPath += "Badges/";
            else if (typeof (T) == typeof (AlbumItem))
                applicationPath += "Albums/Images/";

            if (string.IsNullOrWhiteSpace(imageName))
                imageName = ConstHelper.DefaultImageName;

            return applicationPath + imageName + "?mode=" + mode;
        }

        private static void WaitForUnlock(string token)
        {
            for (var i = 0; i < 50; i++)
            {
                Thread.Sleep(100);
                if (!ActiveWork.Contains(token))
                    return;
            }
        }

        private static void Lock(string token)
        {
            WaitForUnlock(token);
            ActiveWork.Add(token);
        }

        private static void Unlock(string token)
        {
            if (ActiveWork.Contains(token))
                ActiveWork.Remove(token);
        }

        public void DeleteImage<T>(string imageName) where T : class
        {
            var physicalPath = ConstHelper.AppPath + "MediaContent\\";

            if (typeof(T) == typeof(User))
                physicalPath += "Avatars\\";
            else if (typeof(T) == typeof(Group))
                physicalPath += "GroupLogos\\";
            else if (typeof(T) == typeof(Badge))
                physicalPath += "Badges\\";
            else if (typeof(T) == typeof(AlbumItem))
                physicalPath += "Albums\\Images\\";

            if (File.Exists(physicalPath + imageName))
                File.Delete(physicalPath + imageName);
        }

        public string CropExistedImage<T>(string imageName, int x1, int y1, int x2, int y2) where T : class
        {
            var physicalPath = ConstHelper.AppPath + "MediaContent\\";

            if (typeof (T) == typeof (User))
                physicalPath += "Avatars\\";
            else if (typeof (T) == typeof (Group))
                physicalPath += "GroupLogos\\";
            else if (typeof (T) == typeof (Badge))
                physicalPath += "Badges\\";
            else if (typeof (T) == typeof (AlbumItem))
                physicalPath += "Albums\\Images\\";

            if (!File.Exists(physicalPath + imageName))
                throw new BusinessLogicException("Файл не найден!");

            Lock(physicalPath + imageName);

            var image = new Bitmap(physicalPath + imageName);

            var destWidth = x2 - x1;
            var destHeight = y2 - y1;

            var sourceRectangle = new RectangleF(x1, y1, destWidth, destHeight);
            var cropedImage = image.Clone(sourceRectangle, image.PixelFormat);
            image.Dispose();

            imageName = Guid.NewGuid() + ".jpeg";
            var path = physicalPath + imageName;

            var codec = ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == "image/jpeg");

            var parameters = new EncoderParameters(3);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            parameters.Param[1] = new EncoderParameter(Encoder.ScanMethod, (int)EncoderValue.ScanMethodInterlaced);
            parameters.Param[2] = new EncoderParameter(Encoder.RenderMethod, (int)EncoderValue.RenderProgressive);

            var stream = new FileStream(path, FileMode.Create);
            cropedImage.Save(stream, codec, parameters);
            cropedImage.Dispose();
            stream.Close();
            stream.Dispose();

            Unlock(physicalPath + imageName);

            return imageName;
        }

        public Size GetImageSize<T>(string imageName) where T : class
        {
            var physicalPath = ConstHelper.AppPath + "MediaContent\\";

            if (typeof(T) == typeof(User))
                physicalPath += "Avatars\\";
            else if (typeof(T) == typeof(Group))
                physicalPath += "GroupLogos\\";
            else if (typeof(T) == typeof(Badge))
                physicalPath += "Badges\\";
            else if (typeof(T) == typeof(AlbumItem))
                physicalPath += "Albums\\Images\\";

            var size = new Size();

            if (File.Exists(physicalPath + imageName))
            {
                var image = new Bitmap(physicalPath + imageName);

                size = new Size
                    {
                        Width = image.Width,
                        Height = image.Height
                    };

                image.Dispose();
            }

            return size;
        }
    }
}