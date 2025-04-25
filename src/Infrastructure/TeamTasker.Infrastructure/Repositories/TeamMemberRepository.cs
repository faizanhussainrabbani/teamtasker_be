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
    /// Repository implementation for TeamMember entity
    /// </summary>
    public class TeamMemberRepository : EfRepository<TeamMember>, ITeamMemberRepository
    {
        private readonly ILogger<TeamMemberRepository> _logger;

        public TeamMemberRepository(ApplicationDbContext dbContext, ILogger<TeamMemberRepository> logger) : base(dbContext, logger)
        {
            _logger = logger;
        }

        public async Task<List<TeamMember>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting team members for team {TeamId}", teamId);

            var teamMembers = await _dbContext.TeamMembers
                .Where(tm => tm.TeamId == teamId)
                .Include(tm => tm.User)
                .Include(tm => tm.Team)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} team members for team {TeamId}", teamMembers.Count, teamId);

            return teamMembers;
        }

        public async Task<List<TeamMember>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting team memberships for user {UserId}", userId);

            var teamMembers = await _dbContext.TeamMembers
                .Where(tm => tm.UserId == userId)
                .Include(tm => tm.Team)
                .Include(tm => tm.User)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} team memberships for user {UserId}", teamMembers.Count, userId);

            return teamMembers;
        }

        public async Task<TeamMember> GetByTeamAndUserIdAsync(int teamId, int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting team member for team {TeamId} and user {UserId}", teamId, userId);

            var teamMember = await _dbContext.TeamMembers
                .Where(tm => tm.TeamId == teamId && tm.UserId == userId)
                .Include(tm => tm.Team)
                .Include(tm => tm.User)
                .FirstOrDefaultAsync(cancellationToken);

            if (teamMember == null)
            {
                _logger.LogWarning("Team member for team {TeamId} and user {UserId} not found", teamId, userId);
            }

            return teamMember;
        }
    }
}
