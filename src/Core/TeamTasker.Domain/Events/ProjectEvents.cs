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
}
