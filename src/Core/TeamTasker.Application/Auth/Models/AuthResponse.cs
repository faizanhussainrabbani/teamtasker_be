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
        /// JWT token
        /// </summary>
        public string Token { get; set; }
    }
}
