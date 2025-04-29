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
        public bool IsActive { get; private set; } = true;

        /// <summary>
        /// Gets the name of the team member from the associated user
        /// </summary>
        public string Name => User?.FullName ?? $"User {UserId}";

        public void UpdateRole(string role)
        {
            Role = role;

            AddDomainEvent(new TeamMemberRoleUpdatedEvent(this));
        }

        /// <summary>
        /// Sets the active status of the team member
        /// </summary>
        public void SetActive(bool isActive)
        {
            if (IsActive != isActive)
            {
                IsActive = isActive;
                AddDomainEvent(new TeamMemberStatusUpdatedEvent(this));
            }
        }
    }
}
