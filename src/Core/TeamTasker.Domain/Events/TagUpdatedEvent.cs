using System;
using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    /// <summary>
    /// Event raised when a tag is updated
    /// </summary>
    public class TagUpdatedEvent : INotification
    {
        public Tag Tag { get; }

        public TagUpdatedEvent(Tag tag)
        {
            Tag = tag;
            OccurredOn = DateTime.UtcNow;
        }

        public DateTime OccurredOn { get; }
    }
}
