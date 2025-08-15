using System;
using TeamTasker.Domain.Events;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// Activity entity representing user activities in the system
    /// </summary>
    public class Activity : BaseEntity
    {
        private Activity() { } // Required by EF Core

        private Activity(int userId, ActivityType action, string target, string? targetType = null, int? targetId = null, int? assigneeId = null, string? comment = null)
        {
            UserId = userId;
            Action = action;
            Target = target;
            TargetType = targetType;
            TargetId = targetId;
            AssigneeId = assigneeId;
            Comment = comment;
            CreatedDate = DateTime.UtcNow;

            AddDomainEvent(new ActivityCreatedEvent(this));
        }

        public int UserId { get; private set; }
        public ActivityType Action { get; private set; }
        public string Target { get; private set; } = string.Empty;
        public string? TargetType { get; private set; }
        public int? TargetId { get; private set; }
        public int? AssigneeId { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedDate { get; private set; }

        // Navigation properties
        public User User { get; private set; } = null!;
        public User? Assignee { get; private set; }

        /// <summary>
        /// Factory method to create a task-related activity
        /// </summary>
        public static Activity CreateTaskActivity(int userId, ActivityType action, Task task, int? assigneeId = null, string? comment = null)
        {
            return new Activity(userId, action, task.Title, "Task", task.Id, assigneeId, comment);
        }

        /// <summary>
        /// Factory method to create a project-related activity
        /// </summary>
        public static Activity CreateProjectActivity(int userId, ActivityType action, Project project, string? comment = null)
        {
            return new Activity(userId, action, project.Name, "Project", project.Id, null, comment);
        }

        /// <summary>
        /// Factory method to create a team-related activity
        /// </summary>
        public static Activity CreateTeamActivity(int userId, ActivityType action, Team team, int? assigneeId = null, string? comment = null)
        {
            return new Activity(userId, action, team.Name, "Team", team.Id, assigneeId, comment);
        }

        /// <summary>
        /// Factory method to create a general activity
        /// </summary>
        public static Activity Create(int userId, ActivityType action, string target, string? targetType = null, int? targetId = null, int? assigneeId = null, string? comment = null)
        {
            return new Activity(userId, action, target, targetType, targetId, assigneeId, comment);
        }
    }

    /// <summary>
    /// Enumeration of activity types
    /// </summary>
    public enum ActivityType
    {
        Created,
        Updated,
        Completed,
        Assigned,
        Commented,
        Deleted
    }
}
