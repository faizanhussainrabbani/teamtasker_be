using System;
using System.Threading;
using System.Threading.Tasks;
using TeamTasker.Domain.Interfaces;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for the Unit of Work pattern
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for the specified entity type
        /// </summary>
        IRepository<TEntity> Repository<TEntity>() where TEntity : TeamTasker.SharedKernel.BaseEntity;

        /// <summary>
        /// Gets the project repository
        /// </summary>
        IProjectRepository ProjectRepository { get; }

        /// <summary>
        /// Gets the task repository
        /// </summary>
        ITaskRepository TaskRepository { get; }

        /// <summary>
        /// Gets the team repository
        /// </summary>
        ITeamRepository TeamRepository { get; }

        /// <summary>
        /// Gets the team member repository
        /// </summary>
        ITeamMemberRepository TeamMemberRepository { get; }

        /// <summary>
        /// Gets the user repository
        /// </summary>
        IUserRepository UserRepository { get; }

        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a transaction on the database
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the transaction
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the transaction
        /// </summary>
        Task RollbackTransactionAsync();
    }
}
