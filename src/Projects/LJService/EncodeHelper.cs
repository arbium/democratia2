using System;
using System.Security.Cryptography;
using System.Text;

namespace LJService
{
    public static class EncodeHelper
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

        public static string Base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                return data;
            }
        }

    }
}
