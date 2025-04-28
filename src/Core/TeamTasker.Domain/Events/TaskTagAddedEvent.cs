using System;
using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    /// <summary>
    /// Event raised when a tag is added to a task
    /// </summary>
    public class TaskTagAddedEvent : INotification
    {
        public TaskTagAddedEvent(Entities.Task task, int tagId)
        {
            Task = task;
            TagId = tagId;
            OccurredOn = DateTime.UtcNow;
        }

        public Entities.Task Task { get; }
        public int TagId { get; }
        public DateTime OccurredOn { get; }
    }
}
