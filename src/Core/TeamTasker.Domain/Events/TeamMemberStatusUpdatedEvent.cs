using System;
using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    /// <summary>
    /// Event raised when a team member's active status is updated
    /// </summary>
    public class TeamMemberStatusUpdatedEvent : INotification
    {
        public TeamMember TeamMember { get; }
        public DateTime OccurredOn { get; }

        public TeamMemberStatusUpdatedEvent(TeamMember teamMember)
        {
            TeamMember = teamMember ?? throw new ArgumentNullException(nameof(teamMember));
            OccurredOn = DateTime.UtcNow;
        }
    }
}
