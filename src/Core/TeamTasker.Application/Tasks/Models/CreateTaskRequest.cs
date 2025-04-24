using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamTasker.Application.Tasks.Models
{
    /// <summary>
    /// Create task request model
    /// </summary>
    public class CreateTaskRequest
    {
        /// <summary>
        /// Task title
        /// </summary>
        [Required]
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
        [Required]
        public string Status { get; set; }
        
        /// <summary>
        /// Task priority (low, medium, high)
        /// </summary>
        [Required]
        public string Priority { get; set; }
        
        /// <summary>
        /// Task due date
        /// </summary>
        public DateTime? DueDate { get; set; }
        
        /// <summary>
        /// Project ID
        /// </summary>
        [Required]
        public int ProjectId { get; set; }
        
        /// <summary>
        /// Assignee ID
        /// </summary>
        public int? AssigneeId { get; set; }
        
        /// <summary>
        /// Task tags
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();
    }
}
