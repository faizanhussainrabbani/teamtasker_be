using System.Threading.Tasks;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Services
{
    /// <summary>
    /// Domain service for project status logic
    /// </summary>
    public interface IProjectStatusService
    {
        /// <summary>
        /// Checks if a project can be completed
        /// </summary>
        Task<bool> CanCompleteProjectAsync(Project project);

        /// <summary>
        /// Calculates the overall progress of a project
        /// </summary>
        Task<int> CalculateProjectProgressAsync(Project project);

        /// <summary>
        /// Updates the project status based on task completion
        /// </summary>
        Task UpdateProjectStatusBasedOnTasksAsync(Project project);
    }
}
