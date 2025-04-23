using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    public class SkillCreatedEvent : INotification
    {
        public Skill Skill { get; }

        public SkillCreatedEvent(Skill skill)
        {
            Skill = skill;
        }
    }

    public class SkillUpdatedEvent : INotification
    {
        public Skill Skill { get; }

        public SkillUpdatedEvent(Skill skill)
        {
            Skill = skill;
        }
    }

    public class UserSkillCreatedEvent : INotification
    {
        public UserSkill UserSkill { get; }

        public UserSkillCreatedEvent(UserSkill userSkill)
        {
            UserSkill = userSkill;
        }
    }

    public class UserSkillUpdatedEvent : INotification
    {
        public UserSkill UserSkill { get; }

        public UserSkillUpdatedEvent(UserSkill userSkill)
        {
            UserSkill = userSkill;
        }
    }
}
