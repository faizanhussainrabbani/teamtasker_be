using System.ComponentModel.DataAnnotations;

namespace TeamTasker.Application.Auth.Models
{
    /// <summary>
    /// Reset password request model
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Reset token
        /// </summary>
        [Required]
        public string Token { get; set; }
        
        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        /// <summary>
        /// New password
        /// </summary>
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        
        /// <summary>
        /// Confirm new password
        /// </summary>
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
