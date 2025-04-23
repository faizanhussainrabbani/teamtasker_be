using System.ComponentModel.DataAnnotations;

namespace TeamTasker.Application.Auth.Models
{
    /// <summary>
    /// Forgot password request model
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
