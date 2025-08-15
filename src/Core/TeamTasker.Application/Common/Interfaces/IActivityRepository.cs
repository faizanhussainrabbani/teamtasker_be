using TeamTasker.Application.Common.Models;
using TeamTasker.Domain.Entities;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Repository interface for Activity entity
    /// </summary>
    public interface IActivityRepository : IRepository<Activity>
    {
        /// <summary>
        /// Get activities with filtering and pagination
        /// </summary>
        Task<PaginatedList<Activity>> GetActivitiesAsync(
            int? userId = null,
            string? targetType = null,
            int? targetId = null,
            ActivityType? action = null,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            string? sortDirection = "desc",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get recent activities for a user
        /// </summary>
        Task<IReadOnlyList<Activity>> GetRecentActivitiesAsync(
            int userId,
            int count = 10,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get activities for a specific target
        /// </summary>
        Task<IReadOnlyList<Activity>> GetActivitiesForTargetAsync(
            string targetType,
            int targetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get team activities (activities by team members)
        /// </summary>
        Task<PaginatedList<Activity>> GetTeamActivitiesAsync(
            int teamId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
    }
}
