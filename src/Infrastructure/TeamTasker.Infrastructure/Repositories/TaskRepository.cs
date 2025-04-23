using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Data;

namespace TeamTasker.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Task entity
    /// </summary>
    public class TaskRepository : EfRepository<Domain.Entities.Task>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Domain.Entities.Task>> GetTasksByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Where(t => t.AssignedToUserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Domain.Entities.Task>> GetTasksByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync(cancellationToken);
        }
    }
}
