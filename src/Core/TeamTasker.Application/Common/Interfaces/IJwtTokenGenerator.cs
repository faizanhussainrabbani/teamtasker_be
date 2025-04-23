using TeamTasker.Domain.Entities;

namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for JWT token generation
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">User to generate token for</param>
        /// <returns>JWT token string</returns>
        string GenerateToken(User user);
    }
}
