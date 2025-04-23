using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    public class TeamCreatedEvent : INotification
    {
        public Team Team { get; }

        public TeamCreatedEvent(Team team)
        {
            Team = team;
        }
    }

    public class TeamUpdatedEvent : INotification
    {
        public Team Team { get; }

        public TeamUpdatedEvent(Team team)
        {
            Team = team;
        }
    }

    public class TeamLeadChangedEvent : INotification
    {
        public Team Team { get; }
        public int LeadId { get; }

        public TeamLeadChangedEvent(Team team, int leadId)
        {
            Team = team;
            LeadId = leadId;
        }
    }

    public class TeamLeadRemovedEvent : INotification
    {
        public Team Team { get; }

        public TeamLeadRemovedEvent(Team team)
        {
            Team = team;
        }
    }

    public class TeamMemberAddedEvent : INotification
    {
        public TeamMember TeamMember { get; }

        public TeamMemberAddedEvent(TeamMember teamMember)
        {
            TeamMember = teamMember;
        }
    }

    public class TeamMemberRemovedEvent : INotification
    {
        public Team Team { get; }
        public int UserId { get; }

        public TeamMemberRemovedEvent(Team team, int userId)
        {
            Team = team;
            UserId = userId;
        }
    }

    public class TeamMemberRoleUpdatedEvent : INotification
    {
        public TeamMember TeamMember { get; }

        public TeamMemberRoleUpdatedEvent(TeamMember teamMember)
        {
            TeamMember = teamMember;
        }
    }
}
