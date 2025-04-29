using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TeamTasker.Domain.Events;
using TeamTasker.Domain.Exceptions;
using TeamTasker.SharedKernel;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// Task entity representing a task in the system
    /// </summary>
    public class Task : BaseEntity, IAggregateRoot
    {
        private Task() { } // Required by EF Core

        private Task(string title, string description, DateTime dueDate, TaskPriority priority, int projectId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Task title cannot be empty");

            if (dueDate < DateTime.UtcNow.Date)
                throw new DomainException("Due date cannot be in the past");

            Title = title;
            Description = description ?? string.Empty;
            DueDate = dueDate;
            Priority = priority;
            Status = TaskStatus.ToDo;
            ProjectId = projectId;
            Progress = 0;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskCreatedEvent(this));
        }

        /// <summary>
        /// Factory method to create a new task
        /// </summary>
        public static Task Create(string title, string description, DateTime dueDate, TaskPriority priority, int projectId)
        {
            return new Task(title, description, dueDate, priority, projectId);
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

        // AssignedToUserId and Creator have been removed in favor of TeamMember relationships
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }

        // Navigation properties
        public ICollection<TaskTag> Tags { get; private set; } = new List<TaskTag>();

        /// <summary>
        /// Updates the task details with validation
        /// </summary>
        public void UpdateDetails(string title, string description, DateTime dueDate, TaskPriority priority)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Task title cannot be empty");

            if (dueDate < DateTime.UtcNow.Date && Status != TaskStatus.Done)
                throw new DomainException("Due date cannot be in the past for incomplete tasks");

            Title = title;
            Description = description ?? string.Empty;
            DueDate = dueDate;
            Priority = priority;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskUpdatedEvent(this));
        }

        /// <summary>
        /// Updates the task progress with validation and automatically updates status
        /// </summary>
        public void UpdateProgress(int progress)
        {
            if (Status == TaskStatus.Blocked || Status == TaskStatus.OnHold || Status == TaskStatus.Cancelled)
                throw new DomainException($"Cannot update progress for a task with status {Status}");

            // Normalize progress value
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

        /// <summary>
        /// Updates the task status with validation and automatically updates progress
        /// </summary>
        public void UpdateStatus(TaskStatus status)
        {
            // Validate status transitions
            if (Status == TaskStatus.Done && status != TaskStatus.Done)
            {
                // Only allow reopening a completed task if it's going back to InProgress
                if (status != TaskStatus.InProgress)
                    throw new DomainException("Completed tasks can only be reopened to In Progress status");
            }

            if (Status == TaskStatus.Cancelled && status != TaskStatus.Cancelled)
                throw new DomainException("Cancelled tasks cannot be reopened");

            // Update progress based on status
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
            else if (status == TaskStatus.Blocked || status == TaskStatus.OnHold)
            {
                // Keep the current progress
            }
            else if (status == TaskStatus.Cancelled)
            {
                // Reset progress for cancelled tasks
                Progress = 0;
            }

            Status = status;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskStatusUpdatedEvent(this));
        }

        /// <summary>
        /// Assigns the task to a team member
        /// </summary>
        public void AssignToTeamMember(int teamMemberId)
        {
            if (teamMemberId <= 0)
                throw new DomainException("Invalid team member ID");

            if (Status == TaskStatus.Cancelled)
                throw new DomainException("Cannot assign a cancelled task");

            AssignedToTeamMemberId = teamMemberId;
            UpdatedDate = DateTime.UtcNow;

            // If task was in ToDo status and is being assigned, automatically move to InProgress
            if (Status == TaskStatus.ToDo)
            {
                Status = TaskStatus.InProgress;
                Progress = Math.Max(1, Progress); // Ensure progress is at least 1%
            }

            AddDomainEvent(new TaskAssignedToTeamMemberEvent(this, teamMemberId));
        }

        /// <summary>
        /// Sets the creator team member for this task
        /// </summary>
        public void SetCreatorTeamMember(int teamMemberId)
        {
            if (teamMemberId <= 0)
                throw new DomainException("Invalid team member ID");

            if (CreatorTeamMemberId.HasValue)
                throw new DomainException("Creator team member has already been set");

            CreatorTeamMemberId = teamMemberId;
            UpdatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Removes the current assignment
        /// </summary>
        public void RemoveAssignment()
        {
            if (!AssignedToTeamMemberId.HasValue)
                throw new DomainException("Task is not currently assigned");

            if (Status == TaskStatus.Done)
                throw new DomainException("Cannot unassign a completed task");

            AssignedToTeamMemberId = null;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskUnassignedEvent(this));
        }

        /// <summary>
        /// Adds a tag to the task
        /// </summary>
        public void AddTag(int tagId)
        {
            if (tagId <= 0)
                throw new DomainException("Invalid tag ID");

            // Check if tag already exists in the collection
            var existingTag = Tags.FirstOrDefault(t => t.TagId == tagId);
            if (existingTag != null)
                throw new DomainException("Tag is already associated with this task");

            // The actual tag addition will be handled by the repository layer
            // We just need to add the domain event here
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskTagAddedEvent(this, tagId));
        }

        /// <summary>
        /// Removes a tag from the task
        /// </summary>
        public void RemoveTag(int tagId)
        {
            if (tagId <= 0)
                throw new DomainException("Invalid tag ID");

            // Check if tag exists in the collection
            var existingTag = Tags.FirstOrDefault(t => t.TagId == tagId);
            if (existingTag == null)
                throw new DomainException("Tag is not associated with this task");

            // The actual tag removal will be handled by the repository layer
            // We just need to add the domain event here
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskTagRemovedEvent(this, tagId));
        }

        /// <summary>
        /// Checks if the task can be completed
        /// </summary>
        public bool CanComplete()
        {
            return Status != TaskStatus.Cancelled && Status != TaskStatus.Blocked;
        }

        /// <summary>
        /// Checks if the task is overdue
        /// </summary>
        public bool IsOverdue()
        {
            return Status != TaskStatus.Done &&
                   Status != TaskStatus.Cancelled &&
                   DueDate.Date < DateTime.UtcNow.Date;
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
