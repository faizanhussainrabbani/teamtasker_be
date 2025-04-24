using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DomainTask = TeamTasker.Domain.Entities.Task;
using DomainTaskStatus = TeamTasker.Domain.Entities.TaskStatus;
using DomainTaskPriority = TeamTasker.Domain.Entities.TaskPriority;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Task entity
    /// </summary>
    public interface ITaskRepository : IRepository<DomainTask>
    {
        /// <summary>
        /// Get tasks by assignee ID
        /// </summary>
        /// <param name="assigneeId">Assignee ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tasks</returns>
        Task<List<DomainTask>> GetByAssigneeIdAsync(int assigneeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get tasks by project ID
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tasks</returns>
        Task<List<DomainTask>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get tasks by status
        /// </summary>
        /// <param name="status">Task status</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tasks</returns>
        Task<List<DomainTask>> GetByStatusAsync(DomainTaskStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get tasks by priority
        /// </summary>
        /// <param name="priority">Task priority</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tasks</returns>
        Task<List<DomainTask>> GetByPriorityAsync(DomainTaskPriority priority, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get tasks by due date
        /// </summary>
        /// <param name="dueDate">Due date</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tasks</returns>
        Task<List<DomainTask>> GetByDueDateAsync(DateTime dueDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get tasks by tag
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tasks</returns>
        Task<List<DomainTask>> GetByTagAsync(string tag, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add tag to task
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <param name="tag">Tag</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        System.Threading.Tasks.Task AddTagAsync(int taskId, string tag, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove tag from task
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <param name="tag">Tag</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        System.Threading.Tasks.Task RemoveTagAsync(int taskId, string tag, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get tasks with filtering and pagination
        /// </summary>
        /// <param name="status">Filter by status</param>
        /// <param name="priority">Filter by priority</param>
        /// <param name="assigneeId">Filter by assignee ID</param>
        /// <param name="projectId">Filter by project ID</param>
        /// <param name="dueDate">Filter by due date</param>
        /// <param name="tag">Filter by tag</param>
        /// <param name="searchTerm">Search term</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Sort by field</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tasks</returns>
        Task<(List<DomainTask> Tasks, int TotalCount)> GetFilteredTasksAsync(
            DomainTaskStatus? status = null,
            DomainTaskPriority? priority = null,
            int? assigneeId = null,
            int? projectId = null,
            DateTime? dueDate = null,
            string? tag = null,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortDirection = "asc",
            CancellationToken cancellationToken = default);
    }
}
