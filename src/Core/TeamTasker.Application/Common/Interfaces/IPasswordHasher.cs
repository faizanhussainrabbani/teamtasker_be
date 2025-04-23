namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for password hashing
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a password
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <returns>Hashed password</returns>
        string HashPassword(string password);
        
        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        /// <param name="password">Password to verify</param>
        /// <param name="hashedPassword">Hashed password</param>
        /// <returns>True if the password matches the hash, false otherwise</returns>
        bool VerifyPassword(string password, string hashedPassword);
    }
}
