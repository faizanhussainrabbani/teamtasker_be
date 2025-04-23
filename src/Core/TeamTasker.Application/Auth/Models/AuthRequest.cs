using System.ComponentModel.DataAnnotations;

namespace TeamTasker.Application.Auth.Models
{
    /// <summary>
    /// Authentication request model
    /// </summary>
    public class AuthRequest
    {
        /// <summary>
        /// Username or email
        /// </summary>
        [Required]
        public string Username { get; set; }
        
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
