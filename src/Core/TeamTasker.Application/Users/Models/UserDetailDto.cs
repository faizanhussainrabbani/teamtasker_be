using System;
using System.Collections.Generic;
using TeamTasker.Domain.ValueObjects;

namespace TeamTasker.Application.Users.Models
{
    /// <summary>
    /// User detail data transfer object
    /// </summary>
    public class UserDetailDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Role
        /// </summary>
        public string Role { get; set; }
        
        /// <summary>
        /// Avatar URL
        /// </summary>
        public string Avatar { get; set; }
        
        /// <summary>
        /// User initials
        /// </summary>
        public string Initials { get; set; }
        
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
        public AddressDto Address { get; set; }
        
        /// <summary>
        /// Skills
        /// </summary>
        public List<UserSkillDto> Skills { get; set; } = new List<UserSkillDto>();
        
        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// Updated date
        /// </summary>
        public DateTime UpdatedDate { get; set; }
    }
    
    /// <summary>
    /// Address data transfer object
    /// </summary>
    public class AddressDto
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
        
        /// <summary>
        /// Create from domain entity
        /// </summary>
        /// <param name="address">Address domain entity</param>
        /// <returns>Address DTO</returns>
        public static AddressDto FromDomain(Address address)
        {
            if (address == null) return null;
            
            return new AddressDto
            {
                Street = address.Street,
                City = address.City,
                State = address.State,
                Country = address.Country,
                ZipCode = address.ZipCode
            };
        }
    }
    
    /// <summary>
    /// User skill data transfer object
    /// </summary>
    public class UserSkillDto
    {
        /// <summary>
        /// Skill ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Skill name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Skill level (0-100)
        /// </summary>
        public int Level { get; set; }
        
        /// <summary>
        /// Years of experience
        /// </summary>
        public int YearsOfExperience { get; set; }
    }
}
