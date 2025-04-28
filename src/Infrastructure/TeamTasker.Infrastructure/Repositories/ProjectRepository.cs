using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(ApplicationDbContext dbContext, ILogger<ProjectRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting projects for user {UserId}", userId);

            // Get the team member IDs for this user
            var teamMemberIds = await _dbContext.TeamMembers
                .Where(tm => tm.UserId == userId)
                .Select(tm => tm.Id)
                .ToListAsync(cancellationToken);

            if (!teamMemberIds.Any())
            {
                _logger.LogInformation("No team memberships found for user {UserId}", userId);
                return new List<Project>();
            }

            // Get projects where the user is assigned to tasks as a team member
            var projects = await _dbContext.Projects
                .Where(p => p.Tasks.Any(t => t.AssignedToTeamMemberId.HasValue && teamMemberIds.Contains(t.AssignedToTeamMemberId.Value)))
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} projects for user {UserId}", projects.Count, userId);

            return projects;
        }

        public async Task<Project> GetProjectWithTasksAsync(int projectId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting project {ProjectId} with tasks", projectId);

            var project = await _dbContext.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

            if (project == null)
            {
                _logger.LogWarning("Project {ProjectId} not found", projectId);
                return null;
            }

            _logger.LogInformation("Retrieved project {ProjectId} with {TaskCount} tasks", projectId, project.Tasks.Count);

            return project;
        }
    }
}
