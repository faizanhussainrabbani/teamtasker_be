using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Domain.Entities;
using TeamTasker.Infrastructure.Data;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Activity entity
    /// </summary>
    public class ActivityRepository : EfRepository<Activity>, IActivityRepository
    {
        public ActivityRepository(ApplicationDbContext context, ILogger<EfRepository<Activity>>? logger = null) : base(context, logger)
        {
        }

        public async Task<PaginatedList<Activity>> GetActivitiesAsync(
            int? userId = null,
            string? targetType = null,
            int? targetId = null,
            ActivityType? action = null,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            string? sortDirection = "desc",
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Activities
                .Include(a => a.User)
                .Include(a => a.Assignee)
                .AsQueryable();

            // Apply filters
            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (!string.IsNullOrEmpty(targetType))
                query = query.Where(a => a.TargetType == targetType);

            if (targetId.HasValue)
                query = query.Where(a => a.TargetId == targetId.Value);

            if (action.HasValue)
                query = query.Where(a => a.Action == action.Value);

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "action" => sortDirection?.ToLower() == "asc" 
                    ? query.OrderBy(a => a.Action) 
                    : query.OrderByDescending(a => a.Action),
                "target" => sortDirection?.ToLower() == "asc" 
                    ? query.OrderBy(a => a.Target) 
                    : query.OrderByDescending(a => a.Target),
                "user" => sortDirection?.ToLower() == "asc" 
                    ? query.OrderBy(a => a.User.FirstName) 
                    : query.OrderByDescending(a => a.User.FirstName),
                _ => sortDirection?.ToLower() == "asc" 
                    ? query.OrderBy(a => a.CreatedDate) 
                    : query.OrderByDescending(a => a.CreatedDate)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<Activity>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<IReadOnlyList<Activity>> GetRecentActivitiesAsync(
            int userId,
            int count = 10,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .Include(a => a.User)
                .Include(a => a.Assignee)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedDate)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Activity>> GetActivitiesForTargetAsync(
            string targetType,
            int targetId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .Include(a => a.User)
                .Include(a => a.Assignee)
                .Where(a => a.TargetType == targetType && a.TargetId == targetId)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<PaginatedList<Activity>> GetTeamActivitiesAsync(
            int teamId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            // Get team member user IDs
            var teamMemberIds = await _dbContext.TeamMembers
                .Where(tm => tm.TeamId == teamId)
                .Select(tm => tm.UserId)
                .ToListAsync(cancellationToken);

            var query = _dbContext.Activities
                .Include(a => a.User)
                .Include(a => a.Assignee)
                .Where(a => teamMemberIds.Contains(a.UserId))
                .OrderByDescending(a => a.CreatedDate);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<Activity>(items, totalCount, pageNumber, pageSize);
        }
    }
}
