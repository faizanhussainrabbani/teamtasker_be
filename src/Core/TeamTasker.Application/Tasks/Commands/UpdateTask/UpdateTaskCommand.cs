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

namespace TeamTasker.Application.Tasks.Commands.UpdateTask
{
    /// <summary>
    /// Command to update a task.
    /// </summary>
    public class UpdateTaskCommand : IRequest<TaskDto>
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? DueDate { get; set; }
        public string? AssigneeId { get; set; }
        public List<string>? Tags { get; set; }
        public int? Progress { get; set; }
    }

    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateTaskCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
            {
                throw new NotFoundException(nameof(DomainTask), request.Id);
            }

            var nextTitle = request.Title ?? task.Title;
            var nextDescription = request.Description ?? task.Description;
            var nextDueDate = ResolveDueDate(request.DueDate, task.DueDate);
            var nextPriority = ResolvePriority(request.Priority, task.Priority);

            task.UpdateDetails(nextTitle, nextDescription, nextDueDate, nextPriority);

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!TaskContractMapper.TryParseStatus(request.Status, out var parsedStatus))
                {
                    throw new BadRequestException("Invalid task status value.");
                }

                task.UpdateStatus(parsedStatus);
            }

            if (request.Progress.HasValue)
            {
                task.UpdateProgress(request.Progress.Value);
            }

            if (request.AssigneeId != null)
            {
                await ApplyAssignmentAsync(task, request.AssigneeId, cancellationToken);
            }

            if (request.Tags != null)
            {
                ReplaceTags(task.Id, request.Tags);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            var updatedTask = await LoadTaskAsync(task.Id, cancellationToken);
            return TaskContractMapper.ToDto(updatedTask);
        }

        private static DateTime ResolveDueDate(string dueDate, DateTime currentDueDate)
        {
            if (dueDate == null)
            {
                return currentDueDate;
            }

            if (DateTime.TryParse(dueDate, out var parsedDueDate))
            {
                return DateTime.SpecifyKind(parsedDueDate, DateTimeKind.Utc);
            }

            throw new BadRequestException("Invalid due date value.");
        }

        private static TaskPriority ResolvePriority(string priority, TaskPriority currentPriority)
        {
            if (priority == null)
            {
                return currentPriority;
            }

            if (TaskContractMapper.TryParsePriority(priority, out var parsedPriority))
            {
                return parsedPriority;
            }

            throw new BadRequestException("Invalid task priority value.");
        }

        private async SystemTask ApplyAssignmentAsync(DomainTask task, string assigneeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(assigneeId))
            {
                task.RemoveAssignment();
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

        private void ReplaceTags(int taskId, IEnumerable<string> tags)
        {
            var existingTags = _dbContext.TaskTags.Where(tag => tag.TaskId == taskId);
            _dbContext.TaskTags.RemoveRange(existingTags);

            var cleanedTags = tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var tag in cleanedTags)
            {
                _dbContext.TaskTags.Add(new TaskTag(taskId, tag));
            }
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
