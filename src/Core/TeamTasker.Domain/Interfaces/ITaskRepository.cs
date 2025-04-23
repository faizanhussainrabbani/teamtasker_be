using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Task entity
    /// </summary>
    public interface ITaskRepository : IRepository<Entities.Task>
    {
        Task<IEnumerable<Entities.Task>> GetTasksByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Entities.Task>> GetTasksByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    }
}
