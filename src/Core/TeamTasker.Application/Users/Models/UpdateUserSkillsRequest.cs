using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamTasker.Application.Users.Models
{
    /// <summary>
    /// Update user skills request model
    /// </summary>
    public class UpdateUserSkillsRequest
    {
        /// <summary>
        /// List of skills to update
        /// </summary>
        [Required]
        public List<UserSkillUpdateDto> Skills { get; set; } = new List<UserSkillUpdateDto>();
    }
    
    /// <summary>
    /// User skill update data transfer object
    /// </summary>
    public class UserSkillUpdateDto
    {
        /// <summary>
        /// Skill name
        /// </summary>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Skill level (0-100)
        /// </summary>
        [Range(0, 100)]
        public int Level { get; set; }
        
        /// <summary>
        /// Years of experience
        /// </summary>
        [Range(1, 50)]
        public int YearsOfExperience { get; set; }
    }
}
