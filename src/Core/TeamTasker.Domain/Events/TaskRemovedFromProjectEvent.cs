using TeamTasker.Domain.Entities;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Events
{
    /// <summary>
    /// Event raised when a task is removed from a project
    /// </summary>
    public class TaskRemovedFromProjectEvent : DomainEventBase
    {
        public Task Task { get; }
        public Project Project { get; }

        public TaskRemovedFromProjectEvent(Task task, Project project)
        {
            Task = task;
            Project = project;
        }
    }
}
