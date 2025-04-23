using System;
using System.Collections.Generic;
using TeamTasker.Domain.Events;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// Skill entity representing a skill that can be possessed by users
    /// </summary>
    public class Skill : BaseEntity
    {
        private Skill() { } // Required by EF Core

        public Skill(string name, string category, string description = null)
        {
            Name = name;
            Category = category;
            Description = description;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new SkillCreatedEvent(this));
        }

        public string Name { get; private set; }
        public string Category { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        
        // Navigation properties
        public ICollection<UserSkill> UserSkills { get; private set; } = new List<UserSkill>();

        public void Update(string name, string category, string description)
        {
            Name = name;
            Category = category;
            Description = description;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new SkillUpdatedEvent(this));
        }
    }
}
