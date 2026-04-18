using System;

namespace TeamTasker.Application.Teams.Common
{
    /// <summary>
    /// Team list item DTO.
    /// </summary>
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public int? LeadId { get; set; }
        public string LeadName { get; set; }
        public int MemberCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
