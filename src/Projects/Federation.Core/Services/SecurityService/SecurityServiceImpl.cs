using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Federation.Core
{
    [Export(typeof(ISecurityService))]
    public class SecurityServiceImpl : ISecurityService
    {
        public int Version
        {
            get { return 0; }
        }

        public Guid CalcPasswordHash(string password, Guid salt)
        {
            MD5 md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));

            Guid passwordHash = new Guid(data);

            byte[] destByte = new byte[16];
            byte[] guid1Byte = passwordHash.ToByteArray();
            byte[] guid2Byte = salt.ToByteArray();

            for (int i = 0; i < 16; i++)
            {
                destByte[i] = (byte)(guid1Byte[i] ^ guid2Byte[i]);
            }

            return new Guid(destByte); 
        }

        public void GeneratePasswordHash(string password, out Guid hash, out Guid salt)
        {
            salt = Guid.NewGuid();
            MD5 md5Hasher = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));

            Guid passwordHash = new Guid(data);

            byte[] destByte = new byte[16];
            byte[] guid1Byte = passwordHash.ToByteArray();
            byte[] guid2Byte = salt.ToByteArray();

            for (int i = 0; i < 16; i++)
            {
                destByte[i] = (byte)(guid1Byte[i] ^ guid2Byte[i]);
            }

            hash = new Guid(destByte);            
        }

        public User EmailUserValidation(string email, string password)
        {
            var encryptedEmail = CryptographyService.EncryptEmail(email);
            var user = DataService.PerThread.BaseUserSet.OfType<User>().FirstOrDefault(x => x.EncryptedEmail == encryptedEmail);

            if (user == null)
                return null;

            var passwordHash = CalcPasswordHash(password, user.Salt);

            return passwordHash == user.Password ? user : null;
        }

        public User LoginUserValidation(string login, string password)
        {
            var user = DataService.PerThread.BaseUserSet.OfType<User>().FirstOrDefault(x => x.Login == login);
            if (user == null)
                return null;

            var passwordHash = CalcPasswordHash(password, user.Salt);

            return passwordHash == user.Password ? user : null;
        }
    }
}