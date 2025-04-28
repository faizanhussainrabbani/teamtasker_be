using System;
using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    /// <summary>
    /// Event raised when a tag is created
    /// </summary>
    public class TagCreatedEvent : INotification
    {
        public Tag Tag { get; }

        public TagCreatedEvent(Tag tag)
        {
            Tag = tag;
            OccurredOn = DateTime.UtcNow;
        }

        public DateTime OccurredOn { get; }
    }
}
