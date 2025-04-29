using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.SharedKernel;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Infrastructure.Repositories
{
    /// <summary>
    /// Decorator for repositories that adds caching
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class CachedRepositoryDecorator<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IRepository<T> _decoratedRepository;
        private readonly ICachingService _cachingService;
        private readonly ILogger<CachedRepositoryDecorator<T>> _logger;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
        private readonly string _entityName;

        public CachedRepositoryDecorator(
            IRepository<T> decoratedRepository,
            ICachingService cachingService,
            ILogger<CachedRepositoryDecorator<T>> logger)
        {
            _decoratedRepository = decoratedRepository ?? throw new ArgumentNullException(nameof(decoratedRepository));
            _cachingService = cachingService ?? throw new ArgumentNullException(nameof(cachingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _entityName = typeof(T).Name;
        }

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{_entityName}_{id}";
            return await _cachingService.GetOrCreateAsync(
                cacheKey,
                () => _decoratedRepository.GetByIdAsync(id, cancellationToken),
                _cacheExpiration);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{_entityName}_All";
            return await _cachingService.GetOrCreateAsync(
                cacheKey,
                () => _decoratedRepository.ListAllAsync(cancellationToken),
                _cacheExpiration);
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            // For specifications, we don't cache as they can be complex and varied
            return await _decoratedRepository.ListAsync(spec, cancellationToken);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _decoratedRepository.AddAsync(entity, cancellationToken);
            
            // Invalidate cache for the entity list
            await _cachingService.RemoveAsync($"{_entityName}_All");
            
            return result;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _decoratedRepository.UpdateAsync(entity, cancellationToken);
            
            // Invalidate cache for this entity and the entity list
            await _cachingService.RemoveAsync($"{_entityName}_{entity.Id}");
            await _cachingService.RemoveAsync($"{_entityName}_All");
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _decoratedRepository.DeleteAsync(entity, cancellationToken);
            
            // Invalidate cache for this entity and the entity list
            await _cachingService.RemoveAsync($"{_entityName}_{entity.Id}");
            await _cachingService.RemoveAsync($"{_entityName}_All");
        }

        public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            // For specifications, we don't cache as they can be complex and varied
            return await _decoratedRepository.CountAsync(spec, cancellationToken);
        }

        public async Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            // For specifications, we don't cache as they can be complex and varied
            return await _decoratedRepository.FirstOrDefaultAsync(spec, cancellationToken);
        }
    }
}
