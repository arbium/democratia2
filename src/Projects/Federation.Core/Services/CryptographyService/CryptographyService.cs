using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Federation.Core
{
    public class CryptographyService
    {
        static CryptographyService()
        {
        }

        private static ICryptographyService _currentCryptographyService = GetCurrentService();

        private static ICryptographyService GetCurrentService()
        {
            var services = ServiceLocator.GetAllInstances<ICryptographyService>();

            if (services == null || !services.Any())
                return null;

            ICryptographyService currentService = services.FirstOrDefault();
            foreach (var service in services)
            {
                if(service.Version > currentService.Version)
                    currentService = service;
            }
            return currentService;
        }

        public static string EncryptPassword(string stringToConvert)
        {
            return _currentCryptographyService.EncryptPassword(stringToConvert);
        }

        public static string EncryptEmail(string stringToConvert)
        {
            return _currentCryptographyService.EncryptEmail(stringToConvert);
        }

        public static string DecryptEmail(string stringToConvert)
        {
            return _currentCryptographyService.DecryptEmail(stringToConvert);
        }

        public static string EncryptPhone(string stringToConvert)
        {
            return _currentCryptographyService.EncryptPhone(stringToConvert);
        }

        public static string DecryptPhone(string stringToConvert)
        {
            return _currentCryptographyService.DecryptPhone(stringToConvert);
        }
    }
}
