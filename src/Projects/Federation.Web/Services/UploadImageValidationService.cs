using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Federation.Core;

namespace Federation.Web
{
    public static class UploadImageValidationService
    {
        private static readonly Dictionary<string, string> MimeTypeToExtension = new Dictionary<string, string>();

        static UploadImageValidationService()
        {
            MimeTypeToExtension.Add("image/x-png", "png");
            MimeTypeToExtension.Add("image/png", "png");
            MimeTypeToExtension.Add("image/gif", "gif");
            MimeTypeToExtension.Add("image/x-MS-bmp", "bmp");
            MimeTypeToExtension.Add("image/jpeg", "jpg");
            MimeTypeToExtension.Add("image/pjpeg", "jpg");
        }

        public static bool ValidateImageAndGetNewName(HttpPostedFileBase image, out string imageFileName)
        {
            if (image != null && image.ContentLength > 0)
            {
                if (!MimeTypeToExtension.ContainsKey(image.ContentType))
                    throw new ValidationException("Не поддерживаемый формат изображения!");

                var fileName = Guid.NewGuid().ToString();
                var extension = MimeTypeToExtension[image.ContentType];

                imageFileName = fileName + "." + extension;

                return true;
            }

            imageFileName = string.Empty;

            return false;
        }

        public static bool ValidateImageTypeAndGetNewName(WebResponse response, out string imageFileName)
        {
            if (response.ContentLength > 0)
            {
                if (!MimeTypeToExtension.ContainsKey(response.ContentType))
                    throw new ValidationException("Не поддерживаемый формат изображения!");

                var fileName = Guid.NewGuid().ToString();
                var extension = MimeTypeToExtension[response.ContentType];

                imageFileName = fileName + "." + extension;

                return true;
            }

            imageFileName = string.Empty;

            return false;
        }
    }
}