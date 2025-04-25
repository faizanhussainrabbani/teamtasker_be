using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeamTasker.Domain.Entities;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for TeamMember entity
    /// </summary>
    public interface ITeamMemberRepository : IRepository<TeamMember>
    {
        /// <summary>
        /// Get team members by team ID
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of team members</returns>
        Task<List<TeamMember>> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get team members by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of team members</returns>
        Task<List<TeamMember>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get team member by team ID and user ID
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Team member</returns>
        Task<TeamMember> GetByTeamAndUserIdAsync(int teamId, int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ensures a user has at least one team membership
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The user's team member ID</returns>
        Task<int> EnsureUserHasTeamMembershipAsync(int userId, CancellationToken cancellationToken = default);
    }
}
