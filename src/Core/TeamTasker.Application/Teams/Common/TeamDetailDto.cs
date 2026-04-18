using System;
using System.Collections.Generic;

namespace TeamTasker.Application.Teams.Common
{
    /// <summary>
    /// Team detail DTO.
    /// </summary>
    public class TeamDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public int? LeadId { get; set; }
        public string LeadName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>();
    }

    /// <summary>
    /// Team member DTO.
    /// </summary>
    public class TeamMemberDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime JoinedDate { get; set; }
        public string Avatar { get; set; }
        public string Initials { get; set; }
    }
}
