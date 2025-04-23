using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    public class ProjectCreatedEvent : INotification
    {
        public Project Project { get; }

        public ProjectCreatedEvent(Project project)
        {
            Project = project;
        }
    }

    public class ProjectUpdatedEvent : INotification
    {
        public Project Project { get; }

        public ProjectUpdatedEvent(Project project)
        {
            Project = project;
        }
    }

    public class ProjectStatusUpdatedEvent : INotification
    {
        public Project Project { get; }

        public ProjectStatusUpdatedEvent(Project project)
        {
            Project = project;
        }
    }

    public class ProjectAssignedToTeamEvent : INotification
    {
        public Project Project { get; }
        public int TeamId { get; }

        public ProjectAssignedToTeamEvent(Project project, int teamId)
        {
            Project = project;
            TeamId = teamId;
        }
    }

    public class ProjectRemovedFromTeamEvent : INotification
    {
        public Project Project { get; }

        public ProjectRemovedFromTeamEvent(Project project)
        {
            Project = project;
        }
    }
}
