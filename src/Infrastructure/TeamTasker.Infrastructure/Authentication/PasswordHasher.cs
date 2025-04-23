using System;
using System.Security.Cryptography;
using TeamTasker.Application.Common.Interfaces;

namespace TeamTasker.Infrastructure.Authentication
{
    /// <summary>
    /// Implementation of IPasswordHasher using PBKDF2
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int IterationCount = 10000;
        private const int SaltSize = 128 / 8;
        private const int KeySize = 256 / 8;

        /// <summary>
        /// Hashes a password using PBKDF2
        /// </summary>
        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(KeySize);
            }

            // Combine the salt and hash into a single string
            byte[] hashBytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Convert the hashed password from base64 string to bytes
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Extract the salt from the hash
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Hash the password with the extracted salt
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(KeySize);
            }

            // Compare the computed hash with the stored hash
            for (int i = 0; i < KeySize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
