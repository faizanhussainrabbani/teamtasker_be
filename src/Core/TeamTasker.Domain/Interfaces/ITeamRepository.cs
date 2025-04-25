using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeamTasker.Domain.Entities;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Team entity
    /// </summary>
    public interface ITeamRepository : IRepository<Team>
    {
        /// <summary>
        /// Get teams by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of teams</returns>
        Task<List<Team>> GetTeamsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get team with members
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Team with members</returns>
        Task<Team> GetTeamWithMembersAsync(int teamId, CancellationToken cancellationToken = default);
    }
}
