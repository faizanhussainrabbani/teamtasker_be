using System.ComponentModel.DataAnnotations;

namespace TeamTasker.Application.Auth.Models
{
    /// <summary>
    /// Change password request model
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Current password
        /// </summary>
        [Required]
        public string CurrentPassword { get; set; }
        
        /// <summary>
        /// New password
        /// </summary>
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
        
        /// <summary>
        /// Confirm new password
        /// </summary>
        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}
