using System;
using TeamTasker.Domain.Events;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// Task entity representing a task in the system
    /// </summary>
    public class Task : BaseEntity
    {
        private Task() { } // Required by EF Core

        public Task(string title, string description, DateTime dueDate, TaskPriority priority, int projectId)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Status = TaskStatus.ToDo;
            ProjectId = projectId;
            CreatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new TaskCreatedEvent(this));
        }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime DueDate { get; private set; }
        public TaskPriority Priority { get; private set; }
        public TaskStatus Status { get; private set; }
        public int ProjectId { get; private set; }
        public int? AssignedToUserId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }

        public void UpdateDetails(string title, string description, DateTime dueDate, TaskPriority priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            
            AddDomainEvent(new TaskUpdatedEvent(this));
        }

        public void UpdateStatus(TaskStatus status)
        {
            if (status == TaskStatus.Done && Status != TaskStatus.Done)
            {
                CompletedDate = DateTime.UtcNow;
            }
            
            Status = status;
            
            AddDomainEvent(new TaskStatusUpdatedEvent(this));
        }

        public void AssignToUser(int userId)
        {
            AssignedToUserId = userId;
            
            AddDomainEvent(new TaskAssignedEvent(this, userId));
        }

        public void RemoveAssignment()
        {
            AssignedToUserId = null;
            
            AddDomainEvent(new TaskUnassignedEvent(this));
        }
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done,
        Blocked
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}
