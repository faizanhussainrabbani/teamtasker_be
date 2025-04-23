namespace TeamTasker.Application.Common.Models
{
    /// <summary>
    /// Settings for JWT token generation
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Secret key used to sign the JWT token
        /// </summary>
        public string Secret { get; set; }
        
        /// <summary>
        /// Issuer of the JWT token
        /// </summary>
        public string Issuer { get; set; }
        
        /// <summary>
        /// Audience of the JWT token
        /// </summary>
        public string Audience { get; set; }
        
        /// <summary>
        /// Expiry time in minutes
        /// </summary>
        public int ExpiryMinutes { get; set; }
    }
}
