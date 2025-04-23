namespace TeamTasker.Infrastructure.Settings
{
    /// <summary>
    /// Email settings
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// SMTP server
        /// </summary>
        public string SmtpServer { get; set; }
        
        /// <summary>
        /// SMTP port
        /// </summary>
        public int SmtpPort { get; set; }
        
        /// <summary>
        /// SMTP username
        /// </summary>
        public string SmtpUsername { get; set; }
        
        /// <summary>
        /// SMTP password
        /// </summary>
        public string SmtpPassword { get; set; }
        
        /// <summary>
        /// From email address
        /// </summary>
        public string FromEmail { get; set; }
        
        /// <summary>
        /// From display name
        /// </summary>
        public string FromName { get; set; }
        
        /// <summary>
        /// Whether to enable SSL
        /// </summary>
        public bool EnableSsl { get; set; }
    }
}
