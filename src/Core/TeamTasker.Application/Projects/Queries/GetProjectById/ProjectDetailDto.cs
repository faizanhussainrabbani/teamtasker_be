using System;
using System.Collections.Generic;

namespace TeamTasker.Application.Projects.Queries.GetProjectById
{
    /// <summary>
    /// Data transfer object for Project entity with tasks
    /// </summary>
    public class ProjectDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int? AssignedToUserId { get; set; }
    }
}
