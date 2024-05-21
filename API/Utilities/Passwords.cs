using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace API.Utilities
{
    public class HashedPassword
    {
        // Properties to store the hashed password and salt
        public string Hash { get; set; }
        public string Salt { get; set; }

        // Private constructor to initialize the HashedPassword object
        private HashedPassword(string hash, string salt)
        {
            // Assign the provided hash and salt to the corresponding properties
            this.Hash = hash;
            this.Salt = salt;
        }

        // Factory method to generate a HashedPassword object from a plain-text password
        public static HashedPassword FromPassword(string password)
        {
            // Generate a random salt
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }

            // Convert the salt bytes to a base64-encoded string
            string salt = Convert.ToBase64String(saltBytes);

            // Hash the password using SHA256 along with the generated salt
            byte[] hashedBytes;
            using (var sha256 = new SHA256Managed())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, saltedPassword, passwordBytes.Length, saltBytes.Length);
                hashedBytes = sha256.ComputeHash(saltedPassword);
            }

            // Convert the hashed bytes to a base64-encoded string
            string hash = Convert.ToBase64String(hashedBytes);

            // Return a new HashedPassword object initialized with the hash and salt
            return new HashedPassword(hash, salt);
        }

        // Factory method to create a HashedPassword object from an existing hash and salt
        public static HashedPassword FromHashAndSalt(string hash, string salt)
        {
            // Return a new HashedPassword object initialized with the provided hash and salt
            return new HashedPassword(hash, salt);
        }

        // Method to compare a plain-text password with the hashed password stored in the object
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

            // Compare the computed hash with the stored hash and return the result
            return hashedPassword2.Equals(this.Hash);
        }
    }

}
