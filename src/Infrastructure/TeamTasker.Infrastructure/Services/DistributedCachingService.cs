using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Interfaces;

namespace TeamTasker.Infrastructure.Services
{
    /// <summary>
    /// Implementation of caching service using IDistributedCache
    /// </summary>
    public class DistributedCachingService : ICachingService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<DistributedCachingService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

        public DistributedCachingService(
            IDistributedCache cache,
            ILogger<DistributedCachingService> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Gets a cached item by key
        /// </summary>
        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var cachedValue = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cachedValue))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cached value for key {Key}", key);
                return default;
            }
        }

        /// <summary>
        /// Sets a cached item with the specified key
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
                };

                var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
                await _cache.SetStringAsync(key, serializedValue, options);
                _logger.LogDebug("Set cache value for key {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cached value for key {Key}", key);
            }
        }

        /// <summary>
        /// Removes a cached item by key
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogDebug("Removed cache value for key {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached value for key {Key}", key);
            }
        }

        /// <summary>
        /// Gets a cached item by key or creates it using the factory function if it doesn't exist
        /// </summary>
        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
            {
                _logger.LogDebug("Cache hit for key {Key}", key);
                return cachedValue;
            }

            _logger.LogDebug("Cache miss for key {Key}", key);
            var newValue = await factory();
            await SetAsync(key, newValue, expiration);
            return newValue;
        }

        /// <summary>
        /// Refreshes the expiration time of a cached item
        /// </summary>
        public async Task RefreshAsync(string key, TimeSpan? expiration = null)
        {
            try
            {
                var cachedValue = await _cache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(cachedValue))
                {
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
                    };

                    await _cache.SetStringAsync(key, cachedValue, options);
                    _logger.LogDebug("Refreshed cache expiration for key {Key}", key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing cached value for key {Key}", key);
            }
        }
    }
}
