using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Data;

namespace TeamTasker.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Project entity
    /// </summary>
    public class ProjectRepository : EfRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            // This is a simplified implementation. In a real application, you would have a many-to-many relationship
            // between projects and users, and you would query based on that relationship.
            return await _dbContext.Projects
                .Where(p => p.Tasks.Any(t => t.AssignedToUserId == userId))
                .ToListAsync(cancellationToken);
        }

        public async Task<Project> GetProjectWithTasksAsync(int projectId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
        }
    }
}
