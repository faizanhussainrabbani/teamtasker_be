using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamTasker.SharedKernel;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Infrastructure.Data
{
    /// <summary>
    /// Generic repository implementation using Entity Framework Core
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly ILogger<EfRepository<T>> _logger;

        public EfRepository(ApplicationDbContext dbContext, ILogger<EfRepository<T>> logger = null)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public virtual async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Getting {EntityName} with id {Id}", typeof(T).Name, id);

            var entity = await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);

            if (entity == null)
            {
                _logger?.LogWarning("{EntityName} with id {Id} not found", typeof(T).Name, id);
            }

            return entity;
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Getting all {EntityName} entities", typeof(T).Name);

            var entities = await _dbContext.Set<T>().ToListAsync(cancellationToken);

            _logger?.LogInformation("Retrieved {Count} {EntityName} entities", entities.Count, typeof(T).Name);

            return entities;
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).ToListAsync(cancellationToken);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Adding new {EntityName}", typeof(T).Name);

            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger?.LogInformation("Added {EntityName} with id {Id}", typeof(T).Name, entity.Id);

            return entity;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Updating {EntityName} with id {Id}", typeof(T).Name, entity.Id);

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger?.LogInformation("Updated {EntityName} with id {Id}", typeof(T).Name, entity.Id);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Deleting {EntityName} with id {Id}", typeof(T).Name, entity.Id);

            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger?.LogInformation("Deleted {EntityName} with id {Id}", typeof(T).Name, entity.Id);
        }

        public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).CountAsync(cancellationToken);
        }

        public async Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }
    }
}
