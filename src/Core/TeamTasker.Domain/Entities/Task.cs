using System;
using System.Collections.Generic;
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
            Progress = 0;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskCreatedEvent(this));
        }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime DueDate { get; private set; }
        public TaskPriority Priority { get; private set; }
        public TaskStatus Status { get; private set; }
        public int Progress { get; private set; }
        public int ProjectId { get; private set; }
        public Project? Project { get; private set; }
        public int? AssignedToTeamMemberId { get; private set; }
        public TeamMember? AssignedToTeamMember { get; private set; }
        public int? CreatorTeamMemberId { get; set; } // Allow setting for now
        public TeamMember? CreatorTeamMember { get; private set; }

        // Keep these for backward compatibility during migration
        public int? AssignedToUserId { get; private set; }
        public User? AssignedToUser { get; private set; }
        public int CreatorId { get; set; } // Allow setting for now
        public User? Creator { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }

        // Navigation properties
        public ICollection<TaskTag> Tags { get; private set; } = new List<TaskTag>();

        public void UpdateDetails(string title, string description, DateTime dueDate, TaskPriority priority)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskUpdatedEvent(this));
        }

        public void UpdateProgress(int progress)
        {
            if (progress < 0) progress = 0;
            if (progress > 100) progress = 100;

            Progress = progress;
            UpdatedDate = DateTime.UtcNow;

            // Automatically update status based on progress
            if (progress == 0 && Status != TaskStatus.ToDo)
            {
                Status = TaskStatus.ToDo;
            }
            else if (progress > 0 && progress < 100 && Status != TaskStatus.InProgress)
            {
                Status = TaskStatus.InProgress;
            }
            else if (progress == 100 && Status != TaskStatus.Done)
            {
                Status = TaskStatus.Done;
                CompletedDate = DateTime.UtcNow;
            }

            AddDomainEvent(new TaskProgressUpdatedEvent(this));
        }

        public void UpdateStatus(TaskStatus status)
        {
            if (status == TaskStatus.Done && Status != TaskStatus.Done)
            {
                CompletedDate = DateTime.UtcNow;
                Progress = 100;
            }
            else if (status == TaskStatus.InProgress && Progress == 0)
            {
                Progress = 1; // Set to at least 1% when moving to in progress
            }
            else if (status == TaskStatus.ToDo)
            {
                Progress = 0;
            }

            Status = status;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskStatusUpdatedEvent(this));
        }

        public void AssignToUser(int userId)
        {
            AssignedToUserId = userId;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskAssignedEvent(this, userId));
        }

        public void AssignToTeamMember(int teamMemberId)
        {
            AssignedToTeamMemberId = teamMemberId;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskAssignedToTeamMemberEvent(this, teamMemberId));
        }

        public void SetCreatorTeamMember(int teamMemberId)
        {
            CreatorTeamMemberId = teamMemberId;
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveAssignment()
        {
            AssignedToUserId = null;
            AssignedToTeamMemberId = null;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskUnassignedEvent(this));
        }

        public void AddTag(string tag)
        {
            // This will be handled by the repository layer
            // We just need to add the domain event here
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskTagAddedEvent(this, tag));
        }

        public void RemoveTag(string tag)
        {
            // This will be handled by the repository layer
            // We just need to add the domain event here
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskTagRemovedEvent(this, tag));
        }
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done,
        Blocked,
        OnHold,
        Cancelled
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}
