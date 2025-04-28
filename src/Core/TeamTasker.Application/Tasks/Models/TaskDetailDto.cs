using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TeamTasker.Application.Tasks.Models
{
    /// <summary>
    /// Task detail data transfer object
    /// </summary>
    public class TaskDetailDto
    {
        /// <summary>
        /// Task ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Task title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Task description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Task status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Task priority
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Task due date
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Task progress (0-100)
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Project ID
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Project name
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Assignee Team Member ID
        /// </summary>
        public int? AssigneeTeamMemberId { get; set; }

        /// <summary>
        /// Assignee ID (User ID) - Deprecated, use AssigneeTeamMemberId instead
        /// </summary>
        [Obsolete("Use AssigneeTeamMemberId instead")]
        public int? AssigneeId { get; set; }

        /// <summary>
        /// Assignee information
        /// </summary>
        public UserMinimalDto Assignee { get; set; }

        /// <summary>
        /// Creator Team Member ID
        /// </summary>
        public int? CreatorTeamMemberId { get; set; }

        /// <summary>
        /// Creator ID (User ID) - Deprecated, use CreatorTeamMemberId instead
        /// </summary>
        [Obsolete("Use CreatorTeamMemberId instead")]
        public int? CreatorId { get; set; }

        /// <summary>
        /// Creator information
        /// </summary>
        public UserMinimalDto Creator { get; set; }

        /// <summary>
        /// Task tags
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Updated date
        /// </summary>
        public DateTime UpdatedDate { get; set; }

        /// <summary>
        /// Completed date
        /// </summary>
        public DateTime? CompletedDate { get; set; }
    }
}
