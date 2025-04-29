using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Repositories;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Infrastructure.Data
{
    /// <summary>
    /// Implementation of the Unit of Work pattern
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly IDomainEventDispatcher _dispatcher;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        private IProjectRepository _projectRepository;
        private ITaskRepository _taskRepository;
        private ITeamRepository _teamRepository;
        private ITeamMemberRepository _teamMemberRepository;
        private IUserRepository _userRepository;

        public UnitOfWork(
            ApplicationDbContext dbContext,
            ILogger<UnitOfWork> logger,
            IDomainEventDispatcher dispatcher)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public IProjectRepository ProjectRepository => 
            _projectRepository ??= new ProjectRepository(_dbContext, _logger);

        public ITaskRepository TaskRepository => 
            _taskRepository ??= new TaskRepository(_dbContext, _logger);

        public ITeamRepository TeamRepository => 
            _teamRepository ??= new TeamRepository(_dbContext, _logger);

        public ITeamMemberRepository TeamMemberRepository => 
            _teamMemberRepository ??= new TeamMemberRepository(_dbContext, _logger);

        public IUserRepository UserRepository => 
            _userRepository ??= new UserRepository(_dbContext, _logger);

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (!_repositories.ContainsKey(entityType))
            {
                var repositoryType = typeof(EfRepository<>);
                var repositoryInstance = Activator.CreateInstance(
                    repositoryType.MakeGenericType(entityType), 
                    _dbContext, 
                    _logger);

                _repositories.Add(entityType, repositoryInstance);
            }

            return (IRepository<TEntity>)_repositories[entityType];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to database");
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            _logger.LogInformation("Beginning database transaction");
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active transaction to commit");
            }

            try
            {
                _logger.LogInformation("Committing database transaction");
                await _transaction.CommitAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active transaction to rollback");
            }

            try
            {
                _logger.LogInformation("Rolling back database transaction");
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext.Dispose();
        }
    }
}
