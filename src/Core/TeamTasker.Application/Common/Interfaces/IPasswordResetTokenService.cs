using System.Threading.Tasks;

namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for password reset token service
    /// </summary>
    public interface IPasswordResetTokenService
    {
        /// <summary>
        /// Generate a password reset token for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="email">User email</param>
        /// <returns>Reset token</returns>
        Task<string> GeneratePasswordResetTokenAsync(int userId, string email);
        
        /// <summary>
        /// Validate a password reset token
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="email">User email</param>
        /// <param name="token">Reset token</param>
        /// <returns>True if token is valid</returns>
        Task<bool> ValidatePasswordResetTokenAsync(int userId, string email, string token);
    }
}
