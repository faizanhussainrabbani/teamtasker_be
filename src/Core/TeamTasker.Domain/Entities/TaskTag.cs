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

        public TaskTag(int taskId, int tagId)
        {
            TaskId = taskId;
            TagId = tagId;
            CreatedDate = DateTime.UtcNow;
        }

        public int TaskId { get; private set; }
        public Task Task { get; private set; }
        public int TagId { get; private set; }
        public Tag Tag { get; private set; }
        public DateTime CreatedDate { get; private set; }
    }
}
