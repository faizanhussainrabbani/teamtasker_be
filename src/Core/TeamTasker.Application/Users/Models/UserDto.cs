using System;

namespace TeamTasker.Application.Users.Models
{
    /// <summary>
    /// User data transfer object
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Role
        /// </summary>
        public string Role { get; set; }
        
        /// <summary>
        /// Avatar URL
        /// </summary>
        public string Avatar { get; set; }
        
        /// <summary>
        /// User initials
        /// </summary>
        public string Initials { get; set; }
        
        /// <summary>
        /// Department
        /// </summary>
        public string Department { get; set; }
        
        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// Updated date
        /// </summary>
        public DateTime UpdatedDate { get; set; }
    }
}
