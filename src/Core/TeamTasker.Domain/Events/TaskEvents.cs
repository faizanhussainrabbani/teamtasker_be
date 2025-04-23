using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    public class TaskCreatedEvent : INotification
    {
        public Entities.Task TaskItem { get; }

        public TaskCreatedEvent(Entities.Task task)
        {
            TaskItem = task;
        }
    }

    public class TaskUpdatedEvent : INotification
    {
        public Entities.Task TaskItem { get; }

        public TaskUpdatedEvent(Entities.Task task)
        {
            TaskItem = task;
        }
    }

    public class TaskStatusUpdatedEvent : INotification
    {
        public Entities.Task TaskItem { get; }

        public TaskStatusUpdatedEvent(Entities.Task task)
        {
            TaskItem = task;
        }
    }

    public class TaskAssignedEvent : INotification
    {
        public Entities.Task TaskItem { get; }
        public int UserId { get; }

        public TaskAssignedEvent(Entities.Task task, int userId)
        {
            TaskItem = task;
            UserId = userId;
        }
    }

    public class TaskUnassignedEvent : INotification
    {
        public Entities.Task TaskItem { get; }

        public TaskUnassignedEvent(Entities.Task task)
        {
            TaskItem = task;
        }
    }

    public class TaskAddedToProjectEvent : INotification
    {
        public Entities.Task TaskItem { get; }
        public Project Project { get; }

        public TaskAddedToProjectEvent(Entities.Task task, Project project)
        {
            TaskItem = task;
            Project = project;
        }
    }

    public class TaskProgressUpdatedEvent : INotification
    {
        public Entities.Task TaskItem { get; }

        public TaskProgressUpdatedEvent(Entities.Task task)
        {
            TaskItem = task;
        }
    }

    public class TaskTagAddedEvent : INotification
    {
        public Entities.Task TaskItem { get; }
        public string Tag { get; }

        public TaskTagAddedEvent(Entities.Task task, string tag)
        {
            TaskItem = task;
            Tag = tag;
        }
    }

    public class TaskTagRemovedEvent : INotification
    {
        public Entities.Task TaskItem { get; }
        public string Tag { get; }

        public TaskTagRemovedEvent(Entities.Task task, string tag)
        {
            TaskItem = task;
            Tag = tag;
        }
    }
}
