using System.ComponentModel.DataAnnotations;

namespace TeamTasker.Application.Auth.Models
{
    /// <summary>
    /// Registration request model
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// First name
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        
        /// <summary>
        /// Last name
        /// </summary>
        [Required]
        public string LastName { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Username { get; set; }
        
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
