using System;
using TeamTasker.Application.Common.Models;

namespace TeamTasker.Application.Tasks.Models
{
    /// <summary>
    /// Task filter parameters
    /// </summary>
    public class TaskFilterParams : PaginationParams
    {
        /// <summary>
        /// Filter by status
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filter by priority
        /// </summary>
        public string? Priority { get; set; }

        /// <summary>
        /// Filter by assignee ID
        /// </summary>
        public int? AssigneeId { get; set; }

        /// <summary>
        /// Filter by project ID
        /// </summary>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Filter by due date
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Filter by tag
        /// </summary>
        public string? Tag { get; set; }

        /// <summary>
        /// Filter by task type (my, team)
        /// </summary>
        public string? TaskType { get; set; }

        /// <summary>
        /// Include creator information
        /// </summary>
        public bool IncludeCreator { get; set; } = true;

        /// <summary>
        /// Search term for filtering tasks
        /// </summary>
        public string? Search { get; set; }
    }
}
