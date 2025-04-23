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

        public Project(string name, string description, DateTime startDate, DateTime? endDate)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Status = ProjectStatus.NotStarted;
            Tasks = new List<Entities.Task>();

            AddDomainEvent(new ProjectCreatedEvent(this));
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public ProjectStatus Status { get; private set; }
        public List<Entities.Task> Tasks { get; private set; }

        public void UpdateDetails(string name, string description, DateTime startDate, DateTime? endDate)
        {
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;

            AddDomainEvent(new ProjectUpdatedEvent(this));
        }

        public void UpdateStatus(ProjectStatus status)
        {
            Status = status;

            AddDomainEvent(new ProjectStatusUpdatedEvent(this));
        }

        public Entities.Task AddTask(string title, string description, DateTime dueDate, TaskPriority priority)
        {
            var task = new Entities.Task(title, description, dueDate, priority, this.Id);
            Tasks.Add(task);

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
