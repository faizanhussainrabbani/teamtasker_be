using System;
using System.Collections.Generic;
using System.Linq;
using DomainTask = TeamTasker.Domain.Entities.Task;
using TaskPriority = TeamTasker.Domain.Entities.TaskPriority;
using TaskStatus = TeamTasker.Domain.Entities.TaskStatus;

namespace TeamTasker.Application.Tasks.Common
{
    /// <summary>
    /// Transport model for tasks used by frontend clients.
    /// </summary>
    public class TaskDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string DueDate { get; set; }
        public string AssigneeId { get; set; }
        public TaskAssigneeDto Assignee { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public int Progress { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public int ProjectId { get; set; }
    }

    /// <summary>
    /// Transport model for a task assignee.
    /// </summary>
    public class TaskAssigneeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Initials { get; set; }
    }

    public static class TaskContractMapper
    {
        public static TaskDto ToDto(DomainTask task)
        {
            return new TaskDto
            {
                Id = task.Id.ToString(),
                Title = task.Title,
                Description = task.Description,
                Status = ToClientStatus(task.Status),
                Priority = ToClientPriority(task.Priority),
                DueDate = task.DueDate.ToString("O"),
                AssigneeId = task.AssignedToUserId?.ToString(),
                Assignee = task.AssignedToUser == null
                    ? null
                    : new TaskAssigneeDto
                    {
                        Id = task.AssignedToUser.Id.ToString(),
                        Name = task.AssignedToUser.FullName,
                        Avatar = task.AssignedToUser.Avatar,
                        Initials = task.AssignedToUser.Initials
                    },
                Tags = task.Tags?.Select(tag => tag.Tag).ToList() ?? new List<string>(),
                Progress = task.Progress,
                CreatedAt = task.CreatedDate.ToString("O"),
                UpdatedAt = task.UpdatedDate.ToString("O"),
                ProjectId = task.ProjectId
            };
        }

        public static bool TryParseStatus(string status, out TaskStatus parsedStatus)
        {
            parsedStatus = TaskStatus.ToDo;
            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            var normalizedStatus = status.Trim().ToLowerInvariant();
            switch (normalizedStatus)
            {
                case "todo":
                case "to-do":
                case "to_do":
                    parsedStatus = TaskStatus.ToDo;
                    return true;
                case "in-progress":
                case "inprogress":
                case "in_progress":
                    parsedStatus = TaskStatus.InProgress;
                    return true;
                case "completed":
                case "done":
                    parsedStatus = TaskStatus.Done;
                    return true;
                case "blocked":
                    parsedStatus = TaskStatus.Blocked;
                    return true;
                case "on-hold":
                case "onhold":
                    parsedStatus = TaskStatus.OnHold;
                    return true;
                case "cancelled":
                case "canceled":
                    parsedStatus = TaskStatus.Cancelled;
                    return true;
                default:
                    return Enum.TryParse(status, true, out parsedStatus);
            }
        }

        public static bool TryParsePriority(string priority, out TaskPriority parsedPriority)
        {
            parsedPriority = TaskPriority.Medium;
            if (string.IsNullOrWhiteSpace(priority))
            {
                return false;
            }

            var normalizedPriority = priority.Trim().ToLowerInvariant();
            switch (normalizedPriority)
            {
                case "low":
                    parsedPriority = TaskPriority.Low;
                    return true;
                case "medium":
                    parsedPriority = TaskPriority.Medium;
                    return true;
                case "high":
                    parsedPriority = TaskPriority.High;
                    return true;
                case "critical":
                    parsedPriority = TaskPriority.Critical;
                    return true;
                default:
                    return Enum.TryParse(priority, true, out parsedPriority);
            }
        }

        public static string ToClientStatus(TaskStatus status)
        {
            return status switch
            {
                TaskStatus.ToDo => "todo",
                TaskStatus.InProgress => "in-progress",
                TaskStatus.Done => "completed",
                TaskStatus.Blocked => "blocked",
                TaskStatus.OnHold => "on-hold",
                TaskStatus.Cancelled => "cancelled",
                _ => "todo"
            };
        }

        public static string ToClientPriority(TaskPriority priority)
        {
            return priority switch
            {
                TaskPriority.Low => "low",
                TaskPriority.Medium => "medium",
                TaskPriority.High => "high",
                TaskPriority.Critical => "critical",
                _ => "medium"
            };
        }
    }
}
