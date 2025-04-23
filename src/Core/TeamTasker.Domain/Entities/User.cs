using System;
using System.Collections.Generic;
using TeamTasker.Domain.Events;
using TeamTasker.Domain.ValueObjects;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// User entity representing a user in the system
    /// </summary>
    public class User : BaseEntity
    {
        private User() { } // Required by EF Core

        public User(string firstName, string lastName, string email, string username)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Username = username;
            Status = UserStatus.Active;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;

            // Initialize empty properties
            PasswordHash = string.Empty;
            Role = "User";
            Avatar = string.Empty;
            Department = string.Empty;
            Bio = string.Empty;
            Location = string.Empty;
            Phone = string.Empty;

            // Initialize Address with empty values
            Address = new Address(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            // Generate initials from first and last name
            Initials = $"{firstName[0]}{lastName[0]}".ToUpper();

            AddDomainEvent(new UserCreatedEvent(this));
        }

        public User(string firstName, string lastName, string email, string username, string passwordHash, string role = "User")
            : this(firstName, lastName, email, username)
        {
            PasswordHash = passwordHash;
            Role = role;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }
        public string Avatar { get; private set; }
        public string Initials { get; private set; }
        public string Department { get; private set; }
        public string Bio { get; private set; }
        public string Location { get; private set; }
        public string Phone { get; private set; }
        public UserStatus Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        public Address Address { get; private set; }

        // Navigation properties
        public ICollection<UserSkill> Skills { get; private set; } = new List<UserSkill>();
        public ICollection<Task> AssignedTasks { get; private set; } = new List<Task>();
        public ICollection<TeamMember> TeamMemberships { get; private set; } = new List<TeamMember>();

        public string FullName => $"{FirstName} {LastName}";

        public void UpdateProfile(string firstName, string lastName, string email, string department = null, string bio = null, string location = null, string phone = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;

            if (department != null) Department = department;
            if (bio != null) Bio = bio;
            if (location != null) Location = location;
            if (phone != null) Phone = phone;

            // Update initials if name changed
            Initials = $"{firstName[0]}{lastName[0]}".ToUpper();

            UpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void UpdateAddress(Address address)
        {
            Address = address;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new UserAddressUpdatedEvent(this));
        }

        public void UpdateAvatar(string avatarUrl)
        {
            Avatar = avatarUrl;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void UpdatePassword(string passwordHash)
        {
            PasswordHash = passwordHash;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void UpdateRole(string role)
        {
            Role = role;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void Deactivate()
        {
            Status = UserStatus.Inactive;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new UserDeactivatedEvent(this));
        }

        public void Activate()
        {
            Status = UserStatus.Active;
            UpdatedDate = DateTime.UtcNow;

            AddDomainEvent(new UserActivatedEvent(this));
        }
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }
}
