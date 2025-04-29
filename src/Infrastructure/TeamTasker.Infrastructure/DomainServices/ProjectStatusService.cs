using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Services;
using TeamTasker.Infrastructure.Data;
using DomainTask = TeamTasker.Domain.Entities.Task;
using DomainTaskStatus = TeamTasker.Domain.Entities.TaskStatus;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Infrastructure.DomainServices
{
    /// <summary>
    /// Implementation of the project status domain service
    /// </summary>
    public class ProjectStatusService : IProjectStatusService
    {
        private readonly ApplicationDbContext _dbContext;

        public ProjectStatusService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Checks if a project can be completed
        /// </summary>
        public async Task<bool> CanCompleteProjectAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            // A project can be completed if all its tasks are either Done or Cancelled
            var incompleteTasks = await _dbContext.Tasks
                .Where(t => t.ProjectId == project.Id &&
                           t.Status != DomainTaskStatus.Done &&
                           t.Status != DomainTaskStatus.Cancelled)
                .CountAsync();

            return incompleteTasks == 0;
        }

        /// <summary>
        /// Calculates the overall progress of a project
        /// </summary>
        public async Task<int> CalculateProjectProgressAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            // Load tasks if not already loaded
            if (!project.Tasks.Any())
            {
                var tasks = await _dbContext.Tasks
                    .Where(t => t.ProjectId == project.Id)
                    .ToListAsync();

                if (!tasks.Any())
                    return 0;

                return (int)Math.Round(tasks.Average(t => t.Progress));
            }

            return project.CalculateProgress();
        }

        /// <summary>
        /// Updates the project status based on task completion
        /// </summary>
        public async Task UpdateProjectStatusBasedOnTasksAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            // Skip if project is already completed or cancelled
            if (project.Status == ProjectStatus.Completed || project.Status == ProjectStatus.Cancelled)
                return;

            // Load tasks if not already loaded
            var tasks = project.Tasks.Any()
                ? project.Tasks
                : await _dbContext.Tasks.Where(t => t.ProjectId == project.Id).ToListAsync();

            if (!tasks.Any())
                return;

            // Check if all tasks are completed or cancelled
            bool allTasksCompleted = tasks.All(t => t.Status == DomainTaskStatus.Done || t.Status == DomainTaskStatus.Cancelled);

            // Check if any tasks are in progress
            bool anyTaskInProgress = tasks.Any(t => t.Status == DomainTaskStatus.InProgress);

            // Check if all tasks are not started
            bool allTasksNotStarted = tasks.All(t => t.Status == DomainTaskStatus.ToDo);

            // Update project status based on task status
            if (allTasksCompleted)
            {
                project.UpdateStatus(ProjectStatus.Completed);
            }
            else if (anyTaskInProgress)
            {
                project.UpdateStatus(ProjectStatus.InProgress);
            }
            else if (allTasksNotStarted && project.Status != ProjectStatus.NotStarted)
            {
                project.UpdateStatus(ProjectStatus.NotStarted);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
