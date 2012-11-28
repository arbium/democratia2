using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Federation.Core
{
    [Export(typeof(ICryptographyService))]
    public class CryptographyServiceImpl : ICryptographyService
    {
        public int Version
        {
            get { return 0; }
        }

        public string EncryptPassword(string stringToConvert)
        {
            return stringToConvert;
        }

        public string EncryptEmail(string stringToConvert)
        {
            return stringToConvert;
        }

        public string DecryptEmail(string stringToConvert)
        {
            return stringToConvert;
        }

        public string EncryptPhone(string stringToConvert)
        {
            return stringToConvert;
        }

        public string DecryptPhone(string stringToConvert)
        {
            return stringToConvert;
        }
    }
}
