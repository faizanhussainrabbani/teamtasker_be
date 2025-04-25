using System;
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

        /// <summary>
        /// Ensures a user has at least one team membership
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The user's team member ID</returns>
        public async Task<int> EnsureUserHasTeamMembershipAsync(int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Ensuring user {UserId} has a team membership", userId);

            // Check if user already has team memberships
            var existingTeamMemberships = await GetByUserIdAsync(userId, cancellationToken);
            if (existingTeamMemberships.Any())
            {
                _logger.LogInformation("User {UserId} already has {Count} team memberships", userId, existingTeamMemberships.Count);
                return existingTeamMemberships.First().Id;
            }

            // User doesn't have any team memberships, create a default one
            _logger.LogInformation("Creating default team membership for user {UserId}", userId);

            // Check if default team exists
            var defaultTeam = await _dbContext.Teams
                .FirstOrDefaultAsync(t => t.Name == "Default Team" && t.Department == "General", cancellationToken);

            if (defaultTeam == null)
            {
                // Create default team
                _logger.LogInformation("Creating default team");
                defaultTeam = new Team("Default Team", "Default team for users without team memberships", "General");
                await _dbContext.Teams.AddAsync(defaultTeam, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            // Create team membership
            var teamMember = new TeamMember(defaultTeam.Id, userId, "Member");
            await _dbContext.TeamMembers.AddAsync(teamMember, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created team membership {TeamMemberId} for user {UserId}", teamMember.Id, userId);

            return teamMember.Id;
        }
    }
}
