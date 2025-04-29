using System;
using System.Collections.Generic;
using System.Linq;
using TeamTasker.Domain.Events;
using TeamTasker.Domain.Exceptions;
using TeamTasker.SharedKernel;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// Project entity representing a project in the system
    /// </summary>
    public class Project : BaseEntity, IAggregateRoot
    {
        private Project() { } // Required by EF Core

        private Project(string name, string description, DateTime startDate, DateTime? endDate, int? teamId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Project name cannot be empty");

            if (startDate.Date < DateTime.UtcNow.Date.AddDays(-30))
                throw new DomainException("Start date cannot be more than 30 days in the past");

            if (endDate.HasValue && endDate.Value.Date < startDate.Date)
                throw new DomainException("End date cannot be earlier than start date");

            Name = name;
            Description = description ?? string.Empty;
            StartDate = startDate;
            EndDate = endDate;
            TeamId = teamId;
            Status = ProjectStatus.NotStarted;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            Tasks = new List<Entities.Task>();

            AddDomainEvent(new ProjectCreatedEvent(this));
        }

        /// <summary>
        /// Factory method to create a new project
        /// </summary>
        public static Project Create(string name, string description, DateTime startDate, DateTime? endDate, int? teamId = null)
        {
            return new Project(name, description, startDate, endDate, teamId);
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public ProjectStatus Status { get; private set; }
        public int? TeamId { get; private set; }
        public Team Team { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        public List<Entities.Task> Tasks { get; private set; }

        /// <summary>
        /// Updates the project details with validation
        /// </summary>
        public void UpdateDetails(string name, string description, DateTime startDate, DateTime? endDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Project name cannot be empty");

            if (Status == ProjectStatus.Cancelled)
                throw new DomainException("Cannot update a cancelled project");

            if (endDate.HasValue && endDate.Value.Date < startDate.Date)
                throw new DomainException("End date cannot be earlier than start date");

            Name = name;
            Description = description ?? string.Empty;
            StartDate = startDate;
            EndDate = endDate;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new ProjectUpdatedEvent(this));
        }

        /// <summary>
        /// Updates the project status with validation
        /// </summary>
        public void UpdateStatus(ProjectStatus status)
        {
            // Validate status transitions
            if (Status == ProjectStatus.Completed && status != ProjectStatus.Completed)
            {
                // Only allow reopening a completed project if it's going back to InProgress
                if (status != ProjectStatus.InProgress)
                    throw new DomainException("Completed projects can only be reopened to In Progress status");
            }

            if (Status == ProjectStatus.Cancelled && status != ProjectStatus.Cancelled)
                throw new DomainException("Cancelled projects cannot be reopened");

            // If moving to Completed, validate that all tasks are done
            if (status == ProjectStatus.Completed && Status != ProjectStatus.Completed)
            {
                var incompleteTasks = Tasks.Where(t => t.Status != TaskStatus.Done && t.Status != TaskStatus.Cancelled).ToList();
                if (incompleteTasks.Any())
                    throw new DomainException($"Cannot complete project with {incompleteTasks.Count} incomplete tasks");
            }

            Status = status;
            UpdatedDate = DateTime.UtcNow;

            // If project is cancelled, cancel all non-completed tasks
            if (status == ProjectStatus.Cancelled)
            {
                foreach (var task in Tasks.Where(t => t.Status != TaskStatus.Done))
                {
                    task.UpdateStatus(TaskStatus.Cancelled);
                }
            }

            AddDomainEvent(new ProjectStatusUpdatedEvent(this));
        }

        /// <summary>
        /// Assigns the project to a team
        /// </summary>
        public void AssignToTeam(int teamId)
        {
            if (teamId <= 0)
                throw new DomainException("Invalid team ID");

            if (Status == ProjectStatus.Cancelled)
                throw new DomainException("Cannot assign a cancelled project");

            TeamId = teamId;
            UpdatedDate = DateTime.UtcNow;

            // If project was not started and is being assigned, automatically move to InProgress
            if (Status == ProjectStatus.NotStarted)
            {
                Status = ProjectStatus.InProgress;
            }

            AddDomainEvent(new ProjectAssignedToTeamEvent(this, teamId));
        }

        /// <summary>
        /// Removes the project from its current team
        /// </summary>
        public void RemoveFromTeam()
        {
            if (!TeamId.HasValue)
                throw new DomainException("Project is not currently assigned to a team");

            if (Status == ProjectStatus.Completed)
                throw new DomainException("Cannot remove a completed project from a team");

            TeamId = null;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new ProjectRemovedFromTeamEvent(this));
        }

        /// <summary>
        /// Adds a new task to the project
        /// </summary>
        public Entities.Task AddTask(string title, string description, DateTime dueDate, TaskPriority priority)
        {
            if (Status == ProjectStatus.Cancelled)
                throw new DomainException("Cannot add tasks to a cancelled project");

            if (Status == ProjectStatus.Completed)
                throw new DomainException("Cannot add tasks to a completed project");

            var task = Entities.Task.Create(title, description, dueDate, priority, this.Id);
            Tasks.Add(task);
            UpdatedDate = DateTime.UtcNow;

            // If project was not started and a task is being added, automatically move to InProgress
            if (Status == ProjectStatus.NotStarted)
            {
                Status = ProjectStatus.InProgress;
            }

            AddDomainEvent(new TaskAddedToProjectEvent(task, this));

            return task;
        }

        /// <summary>
        /// Removes a task from the project
        /// </summary>
        public void RemoveTask(int taskId)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null)
                throw new DomainException($"Task with ID {taskId} not found in project");

            Tasks.Remove(task);
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskRemovedFromProjectEvent(task, this));
        }

        /// <summary>
        /// Calculates the overall progress of the project based on task completion
        /// </summary>
        public int CalculateProgress()
        {
            if (!Tasks.Any())
                return 0;

            return (int)Math.Round(Tasks.Average(t => t.Progress));
        }

        /// <summary>
        /// Checks if the project is overdue
        /// </summary>
        public bool IsOverdue()
        {
            return Status != ProjectStatus.Completed &&
                   Status != ProjectStatus.Cancelled &&
                   EndDate.HasValue &&
                   EndDate.Value.Date < DateTime.UtcNow.Date;
        }
    }

    public enum ProjectStatus
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold,
        Cancelled
    }
}
