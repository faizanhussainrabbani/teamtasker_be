using TeamTasker.Application.Common.Models;

namespace TeamTasker.Application.Users.Models
{
    /// <summary>
    /// User filter parameters
    /// </summary>
    public class UserFilterParams : PaginationParams
    {
        /// <summary>
        /// Filter by role
        /// </summary>
        public string Role { get; set; }
        
        /// <summary>
        /// Filter by department
        /// </summary>
        public string Department { get; set; }
    }
}
