using System;
using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    /// <summary>
    /// Event raised when a task is assigned to a team member
    /// </summary>
    public class TaskAssignedToTeamMemberEvent : INotification
    {
        public TaskAssignedToTeamMemberEvent(Entities.Task task, int teamMemberId)
        {
            Task = task;
            TeamMemberId = teamMemberId;
            OccurredOn = DateTime.UtcNow;
        }

        public Entities.Task Task { get; }
        public int TeamMemberId { get; }
        public DateTime OccurredOn { get; }
    }
}
