using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    public class TaskCreatedEvent : INotification
    {
        public Task Task { get; }

        public TaskCreatedEvent(Task task)
        {
            Task = task;
        }
    }

    public class TaskUpdatedEvent : INotification
    {
        public Task Task { get; }

        public TaskUpdatedEvent(Task task)
        {
            Task = task;
        }
    }

    public class TaskStatusUpdatedEvent : INotification
    {
        public Task Task { get; }

        public TaskStatusUpdatedEvent(Task task)
        {
            Task = task;
        }
    }

    public class TaskAssignedEvent : INotification
    {
        public Task Task { get; }
        public int UserId { get; }

        public TaskAssignedEvent(Task task, int userId)
        {
            Task = task;
            UserId = userId;
        }
    }

    public class TaskUnassignedEvent : INotification
    {
        public Task Task { get; }

        public TaskUnassignedEvent(Task task)
        {
            Task = task;
        }
    }

    public class TaskAddedToProjectEvent : INotification
    {
        public Task Task { get; }
        public Project Project { get; }

        public TaskAddedToProjectEvent(Task task, Project project)
        {
            Task = task;
            Project = project;
        }
    }
}
