using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeamTasker.Domain.Entities;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Project entity
    /// </summary>
    public interface IProjectRepository : IRepository<Project>
    {
        Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Project> GetProjectWithTasksAsync(int projectId, CancellationToken cancellationToken = default);
    }
}
