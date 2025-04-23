using System;
using TeamTasker.Domain.Events;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// TaskTag entity representing a tag associated with a task
    /// </summary>
    public class TaskTag : BaseEntity
    {
        private TaskTag() { } // Required by EF Core

        public TaskTag(int taskId, string tag)
        {
            TaskId = taskId;
            Tag = tag;
            CreatedDate = DateTime.UtcNow;
        }

        public int TaskId { get; private set; }
        public Task Task { get; private set; }
        public string Tag { get; private set; }
        public DateTime CreatedDate { get; private set; }
    }
}
