using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeamTasker.Application.Common.Models;

namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Team DTO
    /// </summary>
    public class TeamDto
    {
        /// <summary>
        /// Team ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Team name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Team description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Team department
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Team lead ID
        /// </summary>
        public int? LeadId { get; set; }

        /// <summary>
        /// Team lead
        /// </summary>
        public UserMinimalDto Lead { get; set; }

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Updated date
        /// </summary>
        public DateTime UpdatedDate { get; set; }
    }

    /// <summary>
    /// Team detail DTO
    /// </summary>
    public class TeamDetailDto : TeamDto
    {
        /// <summary>
        /// Team members
        /// </summary>
        public List<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>();
    }

    /// <summary>
    /// Team member DTO
    /// </summary>
    public class TeamMemberDto
    {
        /// <summary>
        /// Team member ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public UserMinimalDto User { get; set; }

        /// <summary>
        /// Role in the team
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Joined date
        /// </summary>
        public DateTime JoinedDate { get; set; }
    }

    /// <summary>
    /// Create team request
    /// </summary>
    public class CreateTeamRequest
    {
        /// <summary>
        /// Team name
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Team description
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Team department
        /// </summary>
        [StringLength(100)]
        public string Department { get; set; }

        /// <summary>
        /// Team lead ID
        /// </summary>
        public int? LeadId { get; set; }
    }

    /// <summary>
    /// Update team request
    /// </summary>
    public class UpdateTeamRequest
    {
        /// <summary>
        /// Team name
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Team description
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Team department
        /// </summary>
        [StringLength(100)]
        public string Department { get; set; }

        /// <summary>
        /// Team lead ID
        /// </summary>
        public int? LeadId { get; set; }
    }

    /// <summary>
    /// Add team member request
    /// </summary>
    public class AddTeamMemberRequest
    {
        /// <summary>
        /// User ID
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Role in the team
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Member";
    }

    /// <summary>
    /// Update team member request
    /// </summary>
    public class UpdateTeamMemberRequest
    {
        /// <summary>
        /// Role in the team
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Role { get; set; }
    }
}
