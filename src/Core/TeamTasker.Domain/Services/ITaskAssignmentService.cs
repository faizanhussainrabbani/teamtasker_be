using System.Threading.Tasks;
using TeamTasker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Domain.Services
{
    /// <summary>
    /// Domain service for task assignment logic
    /// </summary>
    public interface ITaskAssignmentService
    {
        /// <summary>
        /// Checks if a team member can be assigned to a task
        /// </summary>
        Task<bool> CanAssignTaskToTeamMemberAsync(Entities.Task task, TeamMember teamMember);

        /// <summary>
        /// Assigns a task to a team member with validation
        /// </summary>
        Task AssignTaskToTeamMemberAsync(Entities.Task task, TeamMember teamMember);

        /// <summary>
        /// Gets the workload percentage for a team member
        /// </summary>
        Task<int> GetTeamMemberWorkloadPercentageAsync(int teamMemberId);
    }
}
