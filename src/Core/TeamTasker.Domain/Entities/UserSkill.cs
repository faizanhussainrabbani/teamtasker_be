using System;
using TeamTasker.Domain.Events;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// UserSkill entity representing a skill possessed by a user
    /// </summary>
    public class UserSkill : BaseEntity
    {
        private UserSkill() { } // Required by EF Core

        public UserSkill(int userId, int skillId, int level, int yearsOfExperience, int projects = 0)
        {
            UserId = userId;
            SkillId = skillId;
            Level = level;
            YearsOfExperience = yearsOfExperience;
            Projects = projects;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new UserSkillCreatedEvent(this));
        }

        public int UserId { get; private set; }
        public User User { get; private set; }
        public int SkillId { get; private set; }
        public Skill Skill { get; private set; }
        public int Level { get; private set; }
        public int YearsOfExperience { get; private set; }
        public int Projects { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }

        public void UpdateSkill(int level, int yearsOfExperience, int projects)
        {
            Level = level;
            YearsOfExperience = yearsOfExperience;
            Projects = projects;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new UserSkillUpdatedEvent(this));
        }
    }
}
