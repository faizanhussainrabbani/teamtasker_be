using System;
using System.Threading.Tasks;

namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for caching service
    /// </summary>
    public interface ICachingService
    {
        /// <summary>
        /// Gets a cached item by key
        /// </summary>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Sets a cached item with the specified key
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Removes a cached item by key
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Gets a cached item by key or creates it using the factory function if it doesn't exist
        /// </summary>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

        /// <summary>
        /// Refreshes the expiration time of a cached item
        /// </summary>
        Task RefreshAsync(string key, TimeSpan? expiration = null);
    }
}
