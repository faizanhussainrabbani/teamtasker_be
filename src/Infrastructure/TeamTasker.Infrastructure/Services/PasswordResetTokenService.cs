using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Interfaces;

namespace TeamTasker.Infrastructure.Services
{
    /// <summary>
    /// Service for managing password reset tokens
    /// </summary>
    public class PasswordResetTokenService : IPasswordResetTokenService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<PasswordResetTokenService> _logger;
        private readonly TimeSpan _tokenExpiration = TimeSpan.FromHours(24);

        public PasswordResetTokenService(IDistributedCache cache, ILogger<PasswordResetTokenService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Generate a password reset token for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="email">User email</param>
        /// <returns>Reset token</returns>
        public async Task<string> GeneratePasswordResetTokenAsync(int userId, string email)
        {
            try
            {
                // Generate a random token
                var token = GenerateRandomToken();
                
                // Store the token in the cache with expiration
                var cacheKey = GetCacheKey(userId, email);
                await _cache.SetStringAsync(cacheKey, token, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _tokenExpiration
                });
                
                _logger.LogInformation("Generated password reset token for user {UserId} ({Email})", userId, email);
                
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating password reset token for user {UserId} ({Email})", userId, email);
                throw;
            }
        }

        /// <summary>
        /// Validate a password reset token
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="email">User email</param>
        /// <param name="token">Reset token</param>
        /// <returns>True if token is valid</returns>
        public async Task<bool> ValidatePasswordResetTokenAsync(int userId, string email, string token)
        {
            try
            {
                // Get the token from the cache
                var cacheKey = GetCacheKey(userId, email);
                var storedToken = await _cache.GetStringAsync(cacheKey);
                
                // Check if the token exists and matches
                var isValid = !string.IsNullOrEmpty(storedToken) && storedToken == token;
                
                if (isValid)
                {
                    // Remove the token from the cache after successful validation
                    await _cache.RemoveAsync(cacheKey);
                    _logger.LogInformation("Password reset token validated for user {UserId} ({Email})", userId, email);
                }
                else
                {
                    _logger.LogWarning("Invalid password reset token for user {UserId} ({Email})", userId, email);
                }
                
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating password reset token for user {UserId} ({Email})", userId, email);
                return false;
            }
        }

        /// <summary>
        /// Generate a random token
        /// </summary>
        /// <returns>Random token</returns>
        private string GenerateRandomToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        /// <summary>
        /// Get the cache key for a user's reset token
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="email">User email</param>
        /// <returns>Cache key</returns>
        private string GetCacheKey(int userId, string email)
        {
            return $"PasswordReset:{userId}:{email.ToLower()}";
        }
    }
}
