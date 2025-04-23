using System;
using TeamTasker.Domain.Events;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// TeamMember entity representing a member of a team
    /// </summary>
    public class TeamMember : BaseEntity
    {
        private TeamMember() { } // Required by EF Core

        public TeamMember(int teamId, int userId, string role = "Member")
        {
            TeamId = teamId;
            UserId = userId;
            Role = role;
            JoinedDate = DateTime.UtcNow;
            
            AddDomainEvent(new TeamMemberAddedEvent(this));
        }

        public int TeamId { get; private set; }
        public Team Team { get; private set; }
        public int UserId { get; private set; }
        public User User { get; private set; }
        public string Role { get; private set; }
        public DateTime JoinedDate { get; private set; }

        public void UpdateRole(string role)
        {
            Role = role;
            
            AddDomainEvent(new TeamMemberRoleUpdatedEvent(this));
        }
    }
}
