using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Application.Tasks.Models;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Data;
using DomainTaskStatus = TeamTasker.Domain.Entities.TaskStatus;
using DomainTaskPriority = TeamTasker.Domain.Entities.TaskPriority;

namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Controller for task management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<TasksController> _logger;
        private readonly IApplicationDbContext _dbContext;

        /// <summary>
        /// Constructor
        /// </summary>
        public TasksController(
            ITaskRepository taskRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            ITeamMemberRepository teamMemberRepository,
            ICurrentUserService currentUserService,
            ILogger<TasksController> logger,
            IApplicationDbContext dbContext)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _teamMemberRepository = teamMemberRepository;
            _currentUserService = currentUserService;
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get a list of tasks with optional filtering
        /// </summary>
        /// <param name="filterParams">Filter parameters</param>
        /// <returns>List of tasks</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedList<TaskDto>>> GetTasks([FromQuery] TaskFilterParams filterParams)
        {
            try
            {
                _logger.LogInformation("Getting tasks with filters. TaskType: {TaskType}", filterParams.TaskType);

                // Get current user ID
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Parse status and priority
                DomainTaskStatus? status = null;
                if (!string.IsNullOrEmpty(filterParams.Status))
                {
                    // Handle different status values
                    switch (filterParams.Status.ToLower())
                    {
                        case "todo":
                            status = DomainTaskStatus.ToDo;
                            break;
                        case "in-progress":
                        case "inprogress":
                            status = DomainTaskStatus.InProgress;
                            break;
                        case "completed":
                        case "done":
                            status = DomainTaskStatus.Done;
                            break;
                        case "blocked":
                            status = DomainTaskStatus.Blocked;
                            break;
                        case "on-hold":
                        case "onhold":
                            status = DomainTaskStatus.OnHold;
                            break;
                        case "cancelled":
                            status = DomainTaskStatus.Cancelled;
                            break;
                    }
                }

                DomainTaskPriority? priority = null;
                if (!string.IsNullOrEmpty(filterParams.Priority))
                {
                    // Handle different priority values
                    switch (filterParams.Priority.ToLower())
                    {
                        case "low":
                            priority = DomainTaskPriority.Low;
                            break;
                        case "medium":
                            priority = DomainTaskPriority.Medium;
                            break;
                        case "high":
                            priority = DomainTaskPriority.High;
                            break;
                        case "critical":
                            priority = DomainTaskPriority.Critical;
                            break;
                    }
                }

                // Get filtered tasks
                var (tasks, totalCount) = await _taskRepository.GetFilteredTasksAsync(
                    status,
                    priority,
                    filterParams.AssigneeId,
                    filterParams.ProjectId,
                    filterParams.DueDate,
                    filterParams.Tag,
                    filterParams.Search,
                    filterParams.PageNumber,
                    filterParams.PageSize,
                    filterParams.SortBy,
                    filterParams.SortDirection ?? "asc",
                    currentUserId.Value,
                    filterParams.TaskType);

                // Map to DTOs
                var taskDtos = tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = MapTaskStatusToString(t.Status),
                    Priority = t.Priority.ToString(),
                    DueDate = t.DueDate,
                    Progress = t.Progress,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project?.Name,
                    // Team member information
                    AssigneeId = t.AssignedToTeamMember?.UserId,
                    AssigneeTeamMemberId = t.AssignedToTeamMemberId,
                    Assignee = t.AssignedToTeamMember?.User != null ? new TeamTasker.Application.Tasks.Models.UserMinimalDto
                    {
                        Id = t.AssignedToTeamMember.User.Id,
                        Name = $"{t.AssignedToTeamMember.User.FirstName} {t.AssignedToTeamMember.User.LastName}",
                        Avatar = t.AssignedToTeamMember.User.Avatar,
                        Initials = t.AssignedToTeamMember.User.Initials,
                        TeamMemberId = t.AssignedToTeamMemberId,
                        TeamId = t.AssignedToTeamMember.TeamId,
                        TeamRole = t.AssignedToTeamMember.Role
                    } : null,
                    // Use team member information for creator
                    CreatorId = t.CreatorTeamMember?.UserId,
                    CreatorTeamMemberId = t.CreatorTeamMemberId,
                    Creator = filterParams.IncludeCreator ?
                        (t.CreatorTeamMember?.User != null ? new TeamTasker.Application.Tasks.Models.UserMinimalDto
                        {
                            Id = t.CreatorTeamMember.User.Id,
                            Name = $"{t.CreatorTeamMember.User.FirstName} {t.CreatorTeamMember.User.LastName}",
                            Avatar = t.CreatorTeamMember.User.Avatar,
                            Initials = t.CreatorTeamMember.User.Initials,
                            TeamMemberId = t.CreatorTeamMemberId,
                            TeamId = t.CreatorTeamMember.TeamId,
                            TeamRole = t.CreatorTeamMember.Role
                        } : null) : null,
                    Tags = t.Tags.Select(tt => tt.Tag.Name).ToList(),
                    CreatedDate = t.CreatedDate,
                    UpdatedDate = t.UpdatedDate,
                    CompletedDate = t.CompletedDate
                }).ToList();

                // Create paginated list
                var result = new PaginatedList<TaskDto>(
                    taskDtos,
                    totalCount,
                    filterParams.PageNumber,
                    filterParams.PageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get a single task by ID
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Task details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TaskDetailDto>> GetTask(int id)
        {
            try
            {
                _logger.LogInformation("Getting task {TaskId}", id);

                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // Map to DTO
                var taskDto = new TaskDetailDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = MapTaskStatusToString(task.Status),
                    Priority = task.Priority.ToString(),
                    DueDate = task.DueDate,
                    Progress = task.Progress,
                    ProjectId = task.ProjectId,
                    ProjectName = task.Project?.Name,
                    // Team member information
                    AssigneeTeamMemberId = task.AssignedToTeamMemberId,
                    AssigneeId = task.AssignedToTeamMember?.UserId,
                    Assignee = task.AssignedToTeamMember?.User != null ? new TeamTasker.Application.Tasks.Models.UserMinimalDto
                    {
                        Id = task.AssignedToTeamMember.User.Id,
                        Name = $"{task.AssignedToTeamMember.User.FirstName} {task.AssignedToTeamMember.User.LastName}",
                        Avatar = task.AssignedToTeamMember.User.Avatar,
                        Initials = task.AssignedToTeamMember.User.Initials,
                        TeamMemberId = task.AssignedToTeamMemberId,
                        TeamId = task.AssignedToTeamMember.TeamId,
                        TeamRole = task.AssignedToTeamMember.Role
                    } : null,
                    // Use team member information
                    CreatorTeamMemberId = task.CreatorTeamMemberId,
                    CreatorId = task.CreatorTeamMember?.UserId,
                    Creator = task.CreatorTeamMember?.User != null ? new TeamTasker.Application.Tasks.Models.UserMinimalDto
                    {
                        Id = task.CreatorTeamMember.User.Id,
                        Name = $"{task.CreatorTeamMember.User.FirstName} {task.CreatorTeamMember.User.LastName}",
                        Avatar = task.CreatorTeamMember.User.Avatar,
                        Initials = task.CreatorTeamMember.User.Initials,
                        TeamMemberId = task.CreatorTeamMemberId,
                        TeamId = task.CreatorTeamMember.TeamId,
                        TeamRole = task.CreatorTeamMember.Role
                    } : null,
                    Tags = task.Tags.Select(tt => tt.Tag.Name).ToList(),
                    CreatedDate = task.CreatedDate,
                    UpdatedDate = task.UpdatedDate,
                    CompletedDate = task.CompletedDate
                };

                return Ok(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        /// <param name="request">Create task request</param>
        /// <returns>Created task</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TaskDetailDto>> CreateTask(CreateTaskRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new task");

                // Get current user ID
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Validate project
                var project = await _projectRepository.GetByIdAsync(request.ProjectId);
                if (project == null)
                {
                    return BadRequest(new { message = "Invalid project ID" });
                }

                // Validate assignee if provided
                if (request.AssigneeId.HasValue)
                {
                    var assignee = await _userRepository.GetByIdAsync(request.AssigneeId.Value);
                    if (assignee == null)
                    {
                        return BadRequest(new { message = "Invalid assignee ID" });
                    }
                }

                // Validate team member if provided
                if (request.TeamMemberId.HasValue)
                {
                    var teamMember = await _dbContext.TeamMembers
                        .Include(tm => tm.User)
                        .FirstOrDefaultAsync(tm => tm.Id == request.TeamMemberId.Value);
                    if (teamMember == null)
                    {
                        return BadRequest(new { message = "Invalid team member ID" });
                    }
                }

                // Parse status and priority
                DomainTaskStatus status;
                switch (request.Status.ToLower())
                {
                    case "todo":
                        status = DomainTaskStatus.ToDo;
                        break;
                    case "in-progress":
                    case "inprogress":
                        status = DomainTaskStatus.InProgress;
                        break;
                    case "completed":
                    case "done":
                        status = DomainTaskStatus.Done;
                        break;
                    case "blocked":
                        status = DomainTaskStatus.Blocked;
                        break;
                    case "on-hold":
                    case "onhold":
                        status = DomainTaskStatus.OnHold;
                        break;
                    case "cancelled":
                        status = DomainTaskStatus.Cancelled;
                        break;
                    default:
                        return BadRequest(new { message = "Invalid status" });
                }

                DomainTaskPriority priority;
                switch (request.Priority.ToLower())
                {
                    case "low":
                        priority = DomainTaskPriority.Low;
                        break;
                    case "medium":
                        priority = DomainTaskPriority.Medium;
                        break;
                    case "high":
                        priority = DomainTaskPriority.High;
                        break;
                    case "critical":
                        priority = DomainTaskPriority.Critical;
                        break;
                    default:
                        return BadRequest(new { message = "Invalid priority" });
                }

                // Create task
                var task = new Domain.Entities.Task(
                    request.Title,
                    request.Description,
                    request.DueDate ?? DateTime.UtcNow.AddDays(7),
                    priority,
                    request.ProjectId);

                // Ensure the creator has a team membership and set it on the task
                var creatorTeamMemberId = await _teamMemberRepository.EnsureUserHasTeamMembershipAsync(currentUserId.Value);
                task.SetCreatorTeamMember(creatorTeamMemberId);

                // CreatorId has been removed in favor of CreatorTeamMemberId

                // Set status if different from default
                if (status != DomainTaskStatus.ToDo)
                {
                    task.UpdateStatus(status);
                }

                // Assign to team member if provided
                if (request.TeamMemberId.HasValue)
                {
                    task.AssignToTeamMember(request.TeamMemberId.Value);
                }
                // Otherwise, ensure the assignee has a team membership if AssigneeId is provided
                else if (request.AssigneeId.HasValue)
                {
                    // Ensure the assignee has a team membership and assign the task to it
                    var assigneeTeamMemberId = await _teamMemberRepository.EnsureUserHasTeamMembershipAsync(request.AssigneeId.Value);
                    task.AssignToTeamMember(assigneeTeamMemberId);

                    // AssignToUser has been removed in favor of AssignToTeamMember
                }

                // Save task
                await _taskRepository.AddAsync(task);

                // Add tags
                if (request.Tags != null && request.Tags.Any())
                {
                    foreach (var tag in request.Tags)
                    {
                        await _taskRepository.AddTagAsync(task.Id, tag);
                    }
                }

                // Get the created task with all related data
                var createdTask = await _taskRepository.GetByIdAsync(task.Id);

                // Map to DTO
                var taskDto = new TaskDetailDto
                {
                    Id = createdTask.Id,
                    Title = createdTask.Title,
                    Description = createdTask.Description,
                    Status = MapTaskStatusToString(createdTask.Status),
                    Priority = createdTask.Priority.ToString(),
                    DueDate = createdTask.DueDate,
                    Progress = createdTask.Progress,
                    ProjectId = createdTask.ProjectId,
                    ProjectName = createdTask.Project?.Name,
                    // Team member information
                    AssigneeTeamMemberId = createdTask.AssignedToTeamMemberId,
                    AssigneeId = createdTask.AssignedToTeamMember?.UserId,
                    Assignee = createdTask.AssignedToTeamMember?.User != null ? new TeamTasker.Application.Tasks.Models.UserMinimalDto
                    {
                        Id = createdTask.AssignedToTeamMember.User.Id,
                        Name = $"{createdTask.AssignedToTeamMember.User.FirstName} {createdTask.AssignedToTeamMember.User.LastName}",
                        Avatar = createdTask.AssignedToTeamMember.User.Avatar,
                        Initials = createdTask.AssignedToTeamMember.User.Initials,
                        TeamMemberId = createdTask.AssignedToTeamMemberId,
                        TeamId = createdTask.AssignedToTeamMember.TeamId,
                        TeamRole = createdTask.AssignedToTeamMember.Role
                    } : null,
                    // Use team member information
                    CreatorTeamMemberId = createdTask.CreatorTeamMemberId,
                    CreatorId = createdTask.CreatorTeamMember?.UserId,
                    Creator = createdTask.CreatorTeamMember?.User != null ? new TeamTasker.Application.Tasks.Models.UserMinimalDto
                    {
                        Id = createdTask.CreatorTeamMember.User.Id,
                        Name = $"{createdTask.CreatorTeamMember.User.FirstName} {createdTask.CreatorTeamMember.User.LastName}",
                        Avatar = createdTask.CreatorTeamMember.User.Avatar,
                        Initials = createdTask.CreatorTeamMember.User.Initials,
                        TeamMemberId = createdTask.CreatorTeamMemberId,
                        TeamId = createdTask.CreatorTeamMember.TeamId,
                        TeamRole = createdTask.CreatorTeamMember.Role
                    } : null,
                    Tags = createdTask.Tags.Select(tt => tt.Tag.Name).ToList(),
                    CreatedDate = createdTask.CreatedDate,
                    UpdatedDate = createdTask.UpdatedDate,
                    CompletedDate = createdTask.CompletedDate
                };

                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="request">Update task request</param>
        /// <returns>Updated task</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TaskDetailDto>> UpdateTask(int id, UpdateTaskRequest request)
        {
            try
            {
                _logger.LogInformation("Updating task {TaskId}", id);

                // Get task
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // Update task details if provided
                if (!string.IsNullOrEmpty(request.Title) || !string.IsNullOrEmpty(request.Description) ||
                    !string.IsNullOrEmpty(request.Priority) || request.DueDate.HasValue)
                {
                    // Parse priority if provided
                    var priority = task.Priority;
                    if (!string.IsNullOrEmpty(request.Priority))
                    {
                        switch (request.Priority.ToLower())
                        {
                            case "low":
                                priority = DomainTaskPriority.Low;
                                break;
                            case "medium":
                                priority = DomainTaskPriority.Medium;
                                break;
                            case "high":
                                priority = DomainTaskPriority.High;
                                break;
                            case "critical":
                                priority = DomainTaskPriority.Critical;
                                break;
                            default:
                                return BadRequest(new { message = "Invalid priority" });
                        }
                    }

                    task.UpdateDetails(
                        request.Title ?? task.Title,
                        request.Description ?? task.Description,
                        request.DueDate ?? task.DueDate,
                        priority);
                }

                // Update status if provided
                if (!string.IsNullOrEmpty(request.Status))
                {
                    DomainTaskStatus status;
                    switch (request.Status.ToLower())
                    {
                        case "todo":
                            status = DomainTaskStatus.ToDo;
                            break;
                        case "in-progress":
                        case "inprogress":
                            status = DomainTaskStatus.InProgress;
                            break;
                        case "completed":
                        case "done":
                            status = DomainTaskStatus.Done;
                            break;
                        case "blocked":
                            status = DomainTaskStatus.Blocked;
                            break;
                        case "on-hold":
                        case "onhold":
                            status = DomainTaskStatus.OnHold;
                            break;
                        case "cancelled":
                            status = DomainTaskStatus.Cancelled;
                            break;
                        default:
                            return BadRequest(new { message = "Invalid status" });
                    }

                    task.UpdateStatus(status);
                }

                // Update progress if provided
                if (request.Progress.HasValue)
                {
                    task.UpdateProgress(request.Progress.Value);
                }

                // Update assignee if provided
                if (request.TeamMemberId.HasValue)
                {
                    if (request.TeamMemberId.Value == 0)
                    {
                        // Unassign task
                        task.RemoveAssignment();
                    }
                    else
                    {
                        // Validate team member
                        var teamMember = await _teamMemberRepository.GetByIdAsync(request.TeamMemberId.Value);
                        if (teamMember == null)
                        {
                            return BadRequest(new { message = "Invalid team member ID" });
                        }

                        // Assign task to team member
                        task.AssignToTeamMember(request.TeamMemberId.Value);
                    }
                }
                else if (request.AssigneeId.HasValue)
                {
                    if (request.AssigneeId.Value == 0)
                    {
                        // Unassign task
                        task.RemoveAssignment();
                    }
                    else
                    {
                        // Validate assignee
                        var assignee = await _userRepository.GetByIdAsync(request.AssigneeId.Value);
                        if (assignee == null)
                        {
                            return BadRequest(new { message = "Invalid assignee ID" });
                        }

                        // Ensure the assignee has a team membership and assign the task to it
                        var assigneeTeamMemberId = await _teamMemberRepository.EnsureUserHasTeamMembershipAsync(request.AssigneeId.Value);
                        task.AssignToTeamMember(assigneeTeamMemberId);

                        // AssignToUser has been removed in favor of AssignToTeamMember
                    }
                }

                // Update task
                await _taskRepository.UpdateAsync(task);

                // Update tags if provided
                if (request.Tags != null)
                {
                    // Get current tag names
                    var currentTagNames = task.Tags.Select(t => t.Tag.Name).ToList();

                    // Remove tags that are not in the new list
                    var tagsToRemove = currentTagNames.Except(request.Tags).ToList();
                    foreach (var tagName in tagsToRemove)
                    {
                        await _taskRepository.RemoveTagAsync(task.Id, tagName);
                    }

                    // Add tags that are not in the current list
                    var tagsToAdd = request.Tags.Except(currentTagNames).ToList();
                    foreach (var tagName in tagsToAdd)
                    {
                        await _taskRepository.AddTagAsync(task.Id, tagName);
                    }
                }

                // Get the updated task with all related data
                var updatedTask = await _taskRepository.GetByIdAsync(task.Id);

                // Map to DTO
                var taskDto = new TaskDetailDto
                {
                    Id = updatedTask.Id,
                    Title = updatedTask.Title,
                    Description = updatedTask.Description,
                    Status = MapTaskStatusToString(updatedTask.Status),
                    Priority = updatedTask.Priority.ToString(),
                    DueDate = updatedTask.DueDate,
                    Progress = updatedTask.Progress,
                    ProjectId = updatedTask.ProjectId,
                    ProjectName = updatedTask.Project?.Name,
                    // Team member information
                    AssigneeTeamMemberId = updatedTask.AssignedToTeamMemberId,
                    AssigneeId = updatedTask.AssignedToTeamMember?.UserId,
                    Assignee = updatedTask.AssignedToTeamMember?.User != null ? new TeamTasker.Application.Tasks.Models.UserMinimalDto
                    {
                        Id = updatedTask.AssignedToTeamMember.User.Id,
                        Name = $"{updatedTask.AssignedToTeamMember.User.FirstName} {updatedTask.AssignedToTeamMember.User.LastName}",
                        Avatar = updatedTask.AssignedToTeamMember.User.Avatar,
                        Initials = updatedTask.AssignedToTeamMember.User.Initials,
                        TeamMemberId = updatedTask.AssignedToTeamMemberId,
                        TeamId = updatedTask.AssignedToTeamMember.TeamId,
                        TeamRole = updatedTask.AssignedToTeamMember.Role
                    } : null,
                    // Use team member information
                    CreatorTeamMemberId = updatedTask.CreatorTeamMemberId,
                    CreatorId = updatedTask.CreatorTeamMember?.UserId,
                    Creator = updatedTask.CreatorTeamMember?.User != null ? new TeamTasker.Application.Tasks.Models.UserMinimalDto
                    {
                        Id = updatedTask.CreatorTeamMember.User.Id,
                        Name = $"{updatedTask.CreatorTeamMember.User.FirstName} {updatedTask.CreatorTeamMember.User.LastName}",
                        Avatar = updatedTask.CreatorTeamMember.User.Avatar,
                        Initials = updatedTask.CreatorTeamMember.User.Initials,
                        TeamMemberId = updatedTask.CreatorTeamMemberId,
                        TeamId = updatedTask.CreatorTeamMember.TeamId,
                        TeamRole = updatedTask.CreatorTeamMember.Role
                    } : null,
                    Tags = updatedTask.Tags.Select(tt => tt.Tag.Name).ToList(),
                    CreatedDate = updatedTask.CreatedDate,
                    UpdatedDate = updatedTask.UpdatedDate,
                    CompletedDate = updatedTask.CompletedDate
                };

                return Ok(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteTask(int id)
        {
            try
            {
                _logger.LogInformation("Deleting task {TaskId}", id);

                // Get task
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                // Delete task
                await _taskRepository.DeleteAsync(task);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Map TaskStatus enum to string for API response
        /// </summary>
        private string MapTaskStatusToString(DomainTaskStatus status)
        {
            return status switch
            {
                DomainTaskStatus.ToDo => "todo",
                DomainTaskStatus.InProgress => "in-progress",
                DomainTaskStatus.Done => "completed",
                DomainTaskStatus.Blocked => "blocked",
                DomainTaskStatus.OnHold => "on-hold",
                DomainTaskStatus.Cancelled => "cancelled",
                _ => status.ToString().ToLower()
            };
        }
    }
}
