using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DomainTask = TeamTasker.Domain.Entities.Task;
using DomainTaskStatus = TeamTasker.Domain.Entities.TaskStatus;
using DomainTaskPriority = TeamTasker.Domain.Entities.TaskPriority;
using DomainTaskTag = TeamTasker.Domain.Entities.TaskTag;
using DomainTag = TeamTasker.Domain.Entities.Tag;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Data;

namespace TeamTasker.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Task entity
    /// </summary>
    public class TaskRepository : EfRepository<DomainTask>, ITaskRepository
    {
        // Use constructor from base class
        public TaskRepository(ApplicationDbContext dbContext, ILogger<TaskRepository> logger) : base(dbContext, logger)
        {
        }

        public async Task<List<DomainTask>> GetByAssigneeIdAsync(int assigneeId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting tasks for assignee {AssigneeId}", assigneeId);

            return await _dbContext.Tasks
                .Where(t => t.AssignedToTeamMemberId == assigneeId)
                .Include(t => t.AssignedToTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Project)
                .Include(t => t.CreatorTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DomainTask>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting tasks for project {ProjectId}", projectId);

            return await _dbContext.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.AssignedToTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Project)
                .Include(t => t.CreatorTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DomainTask>> GetByStatusAsync(DomainTaskStatus status, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting tasks with status {Status}", status);

            return await _dbContext.Tasks
                .Where(t => t.Status == status)
                .Include(t => t.AssignedToTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Project)
                .Include(t => t.CreatorTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DomainTask>> GetByPriorityAsync(DomainTaskPriority priority, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting tasks with priority {Priority}", priority);

            return await _dbContext.Tasks
                .Where(t => t.Priority == priority)
                .Include(t => t.AssignedToTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Project)
                .Include(t => t.CreatorTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DomainTask>> GetByDueDateAsync(DateTime dueDate, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting tasks with due date {DueDate}", dueDate.ToShortDateString());

            // Compare only the date part, not the time
            var startDate = dueDate.Date;
            var endDate = startDate.AddDays(1).AddTicks(-1);

            return await _dbContext.Tasks
                .Where(t => t.DueDate >= startDate && t.DueDate <= endDate)
                .Include(t => t.AssignedToTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Project)
                .Include(t => t.CreatorTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DomainTask>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting tasks with tag {Tag}", tag);

            // First find the tag by name
            var tagEntity = await _dbContext.Tags
                .FirstOrDefaultAsync(t => t.Name == tag, cancellationToken);

            if (tagEntity == null)
            {
                return new List<DomainTask>();
            }

            return await _dbContext.Tasks
                .Where(t => t.Tags.Any(tt => tt.TagId == tagEntity.Id))
                .Include(t => t.AssignedToTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Project)
                .Include(t => t.CreatorTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .ToListAsync(cancellationToken);
        }

        public async Task AddTagAsync(int taskId, string tag, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding tag {Tag} to task {TaskId}", tag, taskId);

            var task = await _dbContext.Tasks
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task is null)
            {
                throw new ArgumentException($"Task with ID {taskId} not found");
            }

            // Find or create the tag
            var tagEntity = await _dbContext.Tags
                .FirstOrDefaultAsync(t => t.Name == tag, cancellationToken);

            if (tagEntity == null)
            {
                // Create a new tag
                tagEntity = new DomainTag(tag);
                _dbContext.Tags.Add(tagEntity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            // Check if task already has this tag
            if (!task.Tags.Any(t => t.TagId == tagEntity.Id))
            {
                var taskTag = new DomainTaskTag(taskId, tagEntity.Id);
                task.AddTag(tagEntity.Id);
                _dbContext.TaskTags.Add(taskTag);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task RemoveTagAsync(int taskId, string tag, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing tag {Tag} from task {TaskId}", tag, taskId);

            var task = await _dbContext.Tasks
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task is null)
            {
                throw new ArgumentException($"Task with ID {taskId} not found");
            }

            // Find the tag
            var tagEntity = await _dbContext.Tags
                .FirstOrDefaultAsync(t => t.Name == tag, cancellationToken);

            if (tagEntity == null)
            {
                // Tag doesn't exist, nothing to remove
                return;
            }

            var taskTag = task.Tags.FirstOrDefault(t => t.TagId == tagEntity.Id);
            if (taskTag != null)
            {
                task.RemoveTag(tagEntity.Id);
                _dbContext.TaskTags.Remove(taskTag);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<(List<DomainTask> Tasks, int TotalCount)> GetFilteredTasksAsync(
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
            int? currentUserId = null,
            string? taskType = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting filtered tasks with type {TaskType}", taskType);

            var query = _dbContext.Tasks.AsQueryable();

            // Apply task type filter
            if (currentUserId.HasValue && !string.IsNullOrEmpty(taskType))
            {
                switch (taskType.ToLower())
                {
                    case "my":
                        // My tasks - tasks that were both created by AND assigned to the current user
                        // First, get the team member IDs for the current user
                        var currentUserTeamMemberIds = _dbContext.TeamMembers
                            .Where(tm => tm.UserId == currentUserId.Value)
                            .Select(tm => tm.Id);

                        // Get tasks where the current user is both creator and assignee
                        if (currentUserTeamMemberIds.Any())
                        {
                            // Prioritize team member assignments
                            query = query.Where(t =>
                                // Creator is the current user's team member
                                (t.CreatorTeamMemberId.HasValue && currentUserTeamMemberIds.Contains(t.CreatorTeamMemberId.Value)) &&
                                // Assignee is the current user's team member
                                (t.AssignedToTeamMemberId.HasValue && currentUserTeamMemberIds.Contains(t.AssignedToTeamMemberId.Value)));
                        }
                        else
                        {
                            // If no team members found, user can't have any tasks that match the criteria
                            query = query.Where(t => false); // Empty result set
                        }
                        break;
                    case "team":
                        // Team tasks - tasks assigned to team members in the same teams as the current user (excluding the current user's tasks)
                        // First, get the teams the current user is a member of
                        var userTeamIds = _dbContext.TeamMembers
                            .Where(tm => tm.UserId == currentUserId.Value)
                            .Select(tm => tm.TeamId);

                        // Then, get the team member IDs of all members in those teams (excluding the current user)
                        var teamMemberIds = _dbContext.TeamMembers
                            .Where(tm => userTeamIds.Contains(tm.TeamId) && tm.UserId != currentUserId.Value)
                            .Select(tm => tm.Id);

                        // Finally, get tasks assigned to those team members
                        query = query.Where(t => t.AssignedToTeamMemberId.HasValue && teamMemberIds.Contains(t.AssignedToTeamMemberId.Value));
                        break;
                    case "created":
                        // Tasks created by the current user
                        // First, get the team member IDs for the current user
                        var creatorTeamMemberIds = _dbContext.TeamMembers
                            .Where(tm => tm.UserId == currentUserId.Value)
                            .Select(tm => tm.Id);

                        if (creatorTeamMemberIds.Any())
                        {
                            query = query.Where(t => t.CreatorTeamMemberId.HasValue && creatorTeamMemberIds.Contains(t.CreatorTeamMemberId.Value));
                        }
                        else
                        {
                            // If no team members found, user can't have created any tasks
                            query = query.Where(t => false); // Empty result set
                        }
                        break;
                    case "unassigned":
                        // Unassigned tasks
                        query = query.Where(t => !t.AssignedToTeamMemberId.HasValue);
                        break;
                }
            }

            // Apply status filter
            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority.Value);
            }

            if (assigneeId.HasValue)
            {
                // Check if the assignee ID is a user ID or a team member ID
                var teamMemberIds = _dbContext.TeamMembers
                    .Where(tm => tm.UserId == assigneeId.Value)
                    .Select(tm => tm.Id);

                if (teamMemberIds.Any())
                {
                    // Filter by team member assignments
                    query = query.Where(t => t.AssignedToTeamMemberId.HasValue && teamMemberIds.Contains(t.AssignedToTeamMemberId.Value));
                }
                else
                {
                    // If no team members found for this user, they can't have any tasks assigned
                    query = query.Where(t => false); // Empty result set
                }
            }

            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            if (dueDate.HasValue)
            {
                var startDate = dueDate.Value.Date;
                var endDate = startDate.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.DueDate >= startDate && t.DueDate <= endDate);
            }

            if (!string.IsNullOrEmpty(tag))
            {
                // Find the tag ID first
                var tagId = _dbContext.Tags
                    .Where(t => t.Name == tag)
                    .Select(t => t.Id)
                    .FirstOrDefault();

                if (tagId > 0)
                {
                    query = query.Where(t => t.Tags.Any(tt => tt.TagId == tagId));
                }
                else
                {
                    // Tag doesn't exist, so no tasks can have it
                    query = query.Where(t => false); // Empty result set
                }
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(lowerSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.Description.ToLower().Contains(lowerSearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = ApplySorting(query, sortBy, sortDirection);

            // Apply pagination
            var tasks = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(t => t.AssignedToTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.CreatorTeamMember)
                    .ThenInclude(tm => tm.User)
                .Include(t => t.Project)
                .Include(t => t.Tags)
                    .ThenInclude(tt => tt.Tag)
                .ToListAsync(cancellationToken);

            return (tasks, totalCount);
        }

        private static IQueryable<DomainTask> ApplySorting(IQueryable<DomainTask> query, string? sortBy, string sortDirection)
        {
            var isAscending = string.IsNullOrEmpty(sortDirection) || string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);

            return sortBy?.ToLower() switch
            {
                "title" => isAscending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
                "duedate" => isAscending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate),
                "priority" => isAscending ? query.OrderBy(t => t.Priority) : query.OrderByDescending(t => t.Priority),
                "status" => isAscending ? query.OrderBy(t => t.Status) : query.OrderByDescending(t => t.Status),
                "progress" => isAscending ? query.OrderBy(t => t.Progress) : query.OrderByDescending(t => t.Progress),
                "created" => isAscending ? query.OrderBy(t => t.CreatedDate) : query.OrderByDescending(t => t.CreatedDate),
                "updated" => isAscending ? query.OrderBy(t => t.UpdatedDate) : query.OrderByDescending(t => t.UpdatedDate),
                _ => isAscending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate) // Default sort by due date
            };
        }
    }
}
