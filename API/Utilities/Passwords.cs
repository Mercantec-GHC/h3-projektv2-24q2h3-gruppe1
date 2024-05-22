using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace API.Utilities
{
    public class HashedPassword
    {
        public string Hash { get; set; }
        public string Salt { get; set; }

        private HashedPassword(string hash, string salt)
        {
            this.Hash = hash;
            this.Salt = salt;
        }

        public static HashedPassword FromPassword(string password)
        {
            // Generate a random salt
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }

            byte[] hashedBytes;
            string salt = Convert.ToBase64String(saltBytes);

            using (var sha256 = new SHA256Managed())
            {

                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, saltedPassword, passwordBytes.Length, saltBytes.Length);

                hashedBytes = sha256.ComputeHash(saltedPassword);
            }
            string hash = Convert.ToBase64String(hashedBytes);

            return new HashedPassword(hash, salt);
        }

        public static HashedPassword FromHashAndSalt(string hash, string salt)
        {
            return new HashedPassword(hash, salt);
        }

        public bool Compare(string password)
        {
            byte[] saltBytes = Convert.FromBase64String(this.Salt);

            string hashedPassword2;

            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + this.Salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, saltedPassword, passwordBytes.Length, saltBytes.Length);

                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                hashedPassword2 = Convert.ToBase64String(hashedBytes);
            }

            return hashedPassword2.Equals(this.Hash);
        }
    }
}
