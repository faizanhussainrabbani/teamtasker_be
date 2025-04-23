using System;
using System.Collections.Generic;
using TeamTasker.Domain.Events;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// Team entity representing a team in the system
    /// </summary>
    public class Team : BaseEntity
    {
        private Team() { } // Required by EF Core

        public Team(string name, string description, string department, int? leadId = null)
        {
            Name = name;
            Description = description;
            Department = department;
            LeadId = leadId;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new TeamCreatedEvent(this));
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Department { get; private set; }
        public int? LeadId { get; private set; }
        public User Lead { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        
        // Navigation properties
        public ICollection<TeamMember> Members { get; private set; } = new List<TeamMember>();
        public ICollection<Project> Projects { get; private set; } = new List<Project>();

        public void Update(string name, string description, string department)
        {
            Name = name;
            Description = description;
            Department = department;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new TeamUpdatedEvent(this));
        }

        public void SetLead(int leadId)
        {
            LeadId = leadId;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new TeamLeadChangedEvent(this, leadId));
        }

        public void RemoveLead()
        {
            LeadId = null;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new TeamLeadRemovedEvent(this));
        }
    }
}
