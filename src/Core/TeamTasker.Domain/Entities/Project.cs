using System;
using System.Collections.Generic;
using TeamTasker.Domain.Events;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// Project entity representing a project in the system
    /// </summary>
    public class Project : BaseEntity
    {
        private Project() { } // Required by EF Core

        public Project(string name, string description, DateTime startDate, DateTime? endDate, int? teamId = null)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            TeamId = teamId;
            Status = ProjectStatus.NotStarted;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            Tasks = new List<Entities.Task>();

            AddDomainEvent(new ProjectCreatedEvent(this));
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

        public void UpdateDetails(string name, string description, DateTime startDate, DateTime? endDate)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new ProjectUpdatedEvent(this));
        }

        public void UpdateStatus(ProjectStatus status)
        {
            Status = status;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new ProjectStatusUpdatedEvent(this));
        }

        public void AssignToTeam(int teamId)
        {
            TeamId = teamId;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new ProjectAssignedToTeamEvent(this, teamId));
        }

        public void RemoveFromTeam()
        {
            TeamId = null;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new ProjectRemovedFromTeamEvent(this));
        }

        public Entities.Task AddTask(string title, string description, DateTime dueDate, TaskPriority priority)
        {
            var task = new Entities.Task(title, description, dueDate, priority, this.Id);
            Tasks.Add(task);
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new TaskAddedToProjectEvent(task, this));

            return task;
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
