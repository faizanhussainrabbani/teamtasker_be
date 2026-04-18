using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using TeamTasker.Application.Common.Interfaces;

namespace TeamTasker.Infrastructure.Authentication
{
    /// <summary>
    /// Distributed cache based refresh token service.
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private const string RefreshTokenKeyPrefix = "auth:refresh-token:";
        private const string UserRefreshTokensKeyPrefix = "auth:user-refresh-tokens:";
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        private readonly IDistributedCache _cache;

        public RefreshTokenService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<string> CreateRefreshTokenAsync(int userId, CancellationToken cancellationToken = default)
        {
            var refreshToken = GenerateToken();
            var cacheKey = BuildKey(refreshToken);

            var payload = new RefreshTokenPayload
            {
                UserId = userId,
                ExpiresAtUtc = DateTime.UtcNow.Add(RefreshTokenLifetime)
            };

            var serializedPayload = JsonSerializer.Serialize(payload);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = RefreshTokenLifetime
            };

            await _cache.SetStringAsync(cacheKey, serializedPayload, options, cancellationToken);
            await TrackUserRefreshTokenAsync(userId, refreshToken, cancellationToken);

            return refreshToken;
        }

        public async Task<int?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            var cacheKey = BuildKey(refreshToken);
            var serializedPayload = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (string.IsNullOrWhiteSpace(serializedPayload))
            {
                return null;
            }

            RefreshTokenPayload payload;
            try
            {
                payload = JsonSerializer.Deserialize<RefreshTokenPayload>(serializedPayload);
            }
            catch (JsonException)
            {
                await _cache.RemoveAsync(cacheKey, cancellationToken);
                return null;
            }

            if (payload == null || payload.UserId <= 0 || payload.ExpiresAtUtc <= DateTime.UtcNow)
            {
                await _cache.RemoveAsync(cacheKey, cancellationToken);
                return null;
            }

            return payload.UserId;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return;
            }

            var cacheKey = BuildKey(refreshToken);

            int? userId = null;
            var serializedPayload = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(serializedPayload))
            {
                try
                {
                    var payload = JsonSerializer.Deserialize<RefreshTokenPayload>(serializedPayload);
                    if (payload != null && payload.UserId > 0)
                    {
                        userId = payload.UserId;
                    }
                }
                catch (JsonException)
                {
                    // Ignore malformed payloads during revocation.
                }
            }

            await _cache.RemoveAsync(cacheKey, cancellationToken);

            if (userId.HasValue)
            {
                await UntrackUserRefreshTokenAsync(userId.Value, refreshToken, cancellationToken);
            }
        }

        public async Task RevokeAllRefreshTokensAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
            {
                return;
            }

            var refreshTokens = await GetUserRefreshTokensAsync(userId, cancellationToken);

            foreach (var refreshToken in refreshTokens)
            {
                await _cache.RemoveAsync(BuildKey(refreshToken), cancellationToken);
            }

            await _cache.RemoveAsync(BuildUserTokensKey(userId), cancellationToken);
        }

        private static string BuildKey(string refreshToken)
        {
            return RefreshTokenKeyPrefix + refreshToken;
        }

        private static string BuildUserTokensKey(int userId)
        {
            return UserRefreshTokensKeyPrefix + userId;
        }

        private async Task TrackUserRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(refreshToken))
            {
                return;
            }

            var refreshTokens = await GetUserRefreshTokensAsync(userId, cancellationToken);
            refreshTokens.Add(refreshToken);

            await SaveUserRefreshTokensAsync(userId, refreshTokens, cancellationToken);
        }

        private async Task UntrackUserRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(refreshToken))
            {
                return;
            }

            var refreshTokens = await GetUserRefreshTokensAsync(userId, cancellationToken);
            refreshTokens.Remove(refreshToken);

            await SaveUserRefreshTokensAsync(userId, refreshTokens, cancellationToken);
        }

        private async Task<HashSet<string>> GetUserRefreshTokensAsync(int userId, CancellationToken cancellationToken)
        {
            var key = BuildUserTokensKey(userId);
            var serialized = await _cache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrWhiteSpace(serialized))
            {
                return new HashSet<string>(StringComparer.Ordinal);
            }

            try
            {
                var tokens = JsonSerializer.Deserialize<List<string>>(serialized) ?? new List<string>();

                return new HashSet<string>(
                    tokens.Where(token => !string.IsNullOrWhiteSpace(token)),
                    StringComparer.Ordinal);
            }
            catch (JsonException)
            {
                await _cache.RemoveAsync(key, cancellationToken);
                return new HashSet<string>(StringComparer.Ordinal);
            }
        }

        private async Task SaveUserRefreshTokensAsync(int userId, HashSet<string> refreshTokens, CancellationToken cancellationToken)
        {
            var key = BuildUserTokensKey(userId);

            if (refreshTokens.Count == 0)
            {
                await _cache.RemoveAsync(key, cancellationToken);
                return;
            }

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = RefreshTokenLifetime
            };

            var serialized = JsonSerializer.Serialize(refreshTokens.ToList());
            await _cache.SetStringAsync(key, serialized, options, cancellationToken);
        }

        private static string GenerateToken()
        {
            Span<byte> bytes = stackalloc byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }

        private sealed class RefreshTokenPayload
        {
            public int UserId { get; set; }
            public DateTime ExpiresAtUtc { get; set; }
        }
    }
}
