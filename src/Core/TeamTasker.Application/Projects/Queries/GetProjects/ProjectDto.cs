using System;

namespace TeamTasker.Application.Projects.Queries.GetProjects
{
    /// <summary>
    /// Data transfer object for Project entity
    /// </summary>
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
    }
}
