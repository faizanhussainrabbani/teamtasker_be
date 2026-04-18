using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Tasks.Common;
using TeamTasker.Domain.Entities;
using DomainTask = TeamTasker.Domain.Entities.Task;
using SystemTask = System.Threading.Tasks.Task;
using TaskPriority = TeamTasker.Domain.Entities.TaskPriority;

namespace TeamTasker.Application.Tasks.Commands.CreateTask
{
    /// <summary>
    /// Command to create a task.
    /// </summary>
    public class CreateTaskCommand : IRequest<TaskDto>
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? DueDate { get; set; }
        public string? AssigneeId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public int? ProjectId { get; set; }
    }

    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateTaskCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await ResolveProjectAsync(request.ProjectId, cancellationToken);

            var priority = ResolvePriority(request.Priority);
            var dueDate = ResolveDueDate(request.DueDate);

            var task = project.AddTask(
                request.Title,
                request.Description ?? string.Empty,
                dueDate,
                priority);

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!TaskContractMapper.TryParseStatus(request.Status, out var parsedStatus))
                {
                    throw new BadRequestException("Invalid task status value.");
                }

                task.UpdateStatus(parsedStatus);
            }

            await ApplyAssignmentAsync(task, request.AssigneeId, cancellationToken);

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await ReplaceTagsAsync(task.Id, request.Tags, cancellationToken);

            var createdTask = await LoadTaskAsync(task.Id, cancellationToken);
            return TaskContractMapper.ToDto(createdTask);
        }

        private async Task<Project> ResolveProjectAsync(int? requestedProjectId, CancellationToken cancellationToken)
        {
            if (requestedProjectId.HasValue)
            {
                var requestedProject = await _dbContext.Projects
                    .FirstOrDefaultAsync(project => project.Id == requestedProjectId.Value, cancellationToken);

                if (requestedProject == null)
                {
                    throw new NotFoundException(nameof(Project), requestedProjectId.Value);
                }

                return requestedProject;
            }

            var firstProject = await _dbContext.Projects
                .Where(project => !project.TeamId.HasValue)
                .OrderBy(project => project.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (firstProject != null)
            {
                return firstProject;
            }

            var defaultProject = new Project(
                "General Backlog",
                "Auto-created project for unscoped task submissions.",
                DateTime.UtcNow,
                null);

            _dbContext.Projects.Add(defaultProject);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return defaultProject;
        }

        private static TaskPriority ResolvePriority(string priority)
        {
            if (string.IsNullOrWhiteSpace(priority))
            {
                return TaskPriority.Medium;
            }

            if (TaskContractMapper.TryParsePriority(priority, out var parsedPriority))
            {
                return parsedPriority;
            }

            throw new BadRequestException("Invalid task priority value.");
        }

        private static DateTime ResolveDueDate(string dueDate)
        {
            if (string.IsNullOrWhiteSpace(dueDate))
            {
                return DateTime.UtcNow.AddDays(7);
            }

            if (DateTime.TryParse(dueDate, out var parsedDueDate))
            {
                return DateTime.SpecifyKind(parsedDueDate, DateTimeKind.Utc);
            }

            throw new BadRequestException("Invalid due date value.");
        }

        private async SystemTask ApplyAssignmentAsync(DomainTask task, string assigneeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(assigneeId))
            {
                return;
            }

            if (!int.TryParse(assigneeId, out var parsedAssigneeId))
            {
                throw new BadRequestException("AssigneeId must be a numeric user id.");
            }

            var assigneeExists = await _dbContext.Users
                .AnyAsync(user => user.Id == parsedAssigneeId, cancellationToken);

            if (!assigneeExists)
            {
                throw new NotFoundException(nameof(User), parsedAssigneeId);
            }

            task.AssignToUser(parsedAssigneeId);
        }

        private async SystemTask ReplaceTagsAsync(int taskId, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            if (tags == null)
            {
                return;
            }

            var cleanedTags = tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (cleanedTags.Count == 0)
            {
                return;
            }

            foreach (var tag in cleanedTags)
            {
                _dbContext.TaskTags.Add(new TaskTag(taskId, tag));
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<DomainTask> LoadTaskAsync(int taskId, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .AsNoTracking()
                .Include(t => t.AssignedToUser)
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task == null)
            {
                throw new NotFoundException(nameof(DomainTask), taskId);
            }

            return task;
        }
    }
}
