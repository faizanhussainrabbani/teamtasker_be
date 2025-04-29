using System;
using MediatR;
using TeamTasker.Domain.Entities;
using Task = TeamTasker.Domain.Entities.Task;

namespace TeamTasker.Domain.Events
{
    /// <summary>
    /// Event raised when a task is removed from a project
    /// </summary>
    public class TaskRemovedFromProjectEvent : INotification
    {
        public Task Task { get; }
        public Project Project { get; }
        public DateTime OccurredOn { get; }

        public TaskRemovedFromProjectEvent(Task task, Project project)
        {
            Task = task;
            Project = project;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
