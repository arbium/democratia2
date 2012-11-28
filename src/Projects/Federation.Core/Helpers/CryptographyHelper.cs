using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Federation.Core
{
    public static class CryptographyHelper
    {
        public static string ToMD5(string stringToConvert)
        {
            //create an instance of the MD5CryptoServiceProvider
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();

            //convert our string into byte array
            byte[] byteArray = Encoding.UTF8.GetBytes(stringToConvert);

            //get the hashed values created by our MD5CryptoServiceProvider
            byte[] hashedByteArray = md5Provider.ComputeHash(byteArray);

            //create a StringBuilder object
            StringBuilder stringBuilder = new StringBuilder();

            //loop to each each byte
            foreach (byte b in hashedByteArray)
            {
                //append it to our StringBuilder
                stringBuilder.Append(b.ToString("x2").ToLower());
            }

            //return the hashed value
            return stringBuilder.ToString();
        }
    }
}
