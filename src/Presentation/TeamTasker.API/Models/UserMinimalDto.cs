namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Minimal user DTO for references
    /// </summary>
    public class UserMinimalDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User full name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// User avatar URL
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// User initials
        /// </summary>
        public string Initials { get; set; }

        /// <summary>
        /// Team member ID (if applicable)
        /// </summary>
        public int? TeamMemberId { get; set; }

        /// <summary>
        /// Team ID (if applicable)
        /// </summary>
        public int? TeamId { get; set; }

        /// <summary>
        /// Team role (if applicable)
        /// </summary>
        public string TeamRole { get; set; }
    }
}
