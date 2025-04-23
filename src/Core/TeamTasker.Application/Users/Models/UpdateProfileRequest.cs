using System.ComponentModel.DataAnnotations;

namespace TeamTasker.Application.Users.Models
{
    /// <summary>
    /// Update profile request model
    /// </summary>
    public class UpdateProfileRequest
    {
        /// <summary>
        /// First name
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        
        /// <summary>
        /// Last name
        /// </summary>
        [Required]
        public string LastName { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        /// <summary>
        /// Department
        /// </summary>
        public string Department { get; set; }
        
        /// <summary>
        /// Bio
        /// </summary>
        public string Bio { get; set; }
        
        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// Address
        /// </summary>
        public AddressUpdateRequest Address { get; set; }
    }
    
    /// <summary>
    /// Address update request model
    /// </summary>
    public class AddressUpdateRequest
    {
        /// <summary>
        /// Street
        /// </summary>
        public string Street { get; set; }
        
        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }
        
        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }
        
        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }
        
        /// <summary>
        /// Zip code
        /// </summary>
        public string ZipCode { get; set; }
    }
}
