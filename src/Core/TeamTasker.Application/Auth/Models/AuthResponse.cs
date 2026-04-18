namespace TeamTasker.Application.Auth.Models
{
    /// <summary>
    /// Authentication response model
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Role assigned to the user
        /// </summary>
        public string Role { get; set; }
        
        /// <summary>
        /// JWT token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Access token expiry time in UTC
        /// </summary>
        public System.DateTime? ExpiresAt { get; set; }
    }
}
