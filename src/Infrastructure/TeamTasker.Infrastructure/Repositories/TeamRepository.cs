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
    /// Repository implementation for Team entity
    /// </summary>
    public class TeamRepository : EfRepository<Team>, ITeamRepository
    {
        private readonly ILogger<TeamRepository> _logger;

        public TeamRepository(ApplicationDbContext dbContext, ILogger<TeamRepository> logger) : base(dbContext, logger)
        {
            _logger = logger;
        }

        public async Task<List<Team>> GetTeamsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting teams for user {UserId}", userId);

            var teamIds = await _dbContext.TeamMembers
                .Where(tm => tm.UserId == userId)
                .Select(tm => tm.TeamId)
                .ToListAsync(cancellationToken);

            var teams = await _dbContext.Teams
                .Where(t => teamIds.Contains(t.Id))
                .Include(t => t.Lead)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} teams for user {UserId}", teams.Count, userId);

            return teams;
        }

        public async Task<Team> GetTeamWithMembersAsync(int teamId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting team {TeamId} with members", teamId);

            var team = await _dbContext.Teams
                .Include(t => t.Members)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Lead)
                .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);

            if (team == null)
            {
                _logger.LogWarning("Team {TeamId} not found", teamId);
                return null;
            }

            _logger.LogInformation("Retrieved team {TeamId} with {MemberCount} members", teamId, team.Members.Count);

            return team;
        }
    }
}
