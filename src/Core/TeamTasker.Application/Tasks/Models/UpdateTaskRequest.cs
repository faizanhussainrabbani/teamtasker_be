using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamTasker.Application.Tasks.Models
{
    /// <summary>
    /// Update task request model
    /// </summary>
    public class UpdateTaskRequest
    {
        /// <summary>
        /// Task title
        /// </summary>
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        /// <summary>
        /// Task description
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Task status (todo, in-progress, completed)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Task priority (low, medium, high)
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// Task due date
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Task progress (0-100)
        /// </summary>
        [Range(0, 100)]
        public int? Progress { get; set; }

        /// <summary>
        /// Assignee ID (User ID)
        /// </summary>
        public int? AssigneeId { get; set; }

        /// <summary>
        /// Team Member ID
        /// </summary>
        public int? TeamMemberId { get; set; }

        /// <summary>
        /// Task tags
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
