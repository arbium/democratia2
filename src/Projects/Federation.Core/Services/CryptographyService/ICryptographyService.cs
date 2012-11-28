using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Federation.Core
{
    public interface ICryptographyService
    {
        int Version { get; }

        string EncryptPassword(string stringToConvert);

        string EncryptEmail(string stringToConvert);
        string DecryptEmail(string stringToConvert);
        string EncryptPhone(string stringToConvert);
        string DecryptPhone(string stringToConvert);
    }
}
