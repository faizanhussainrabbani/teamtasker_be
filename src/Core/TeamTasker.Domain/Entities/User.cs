using System;
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
            
            AddDomainEvent(new UserCreatedEvent(this));
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Username { get; private set; }
        public UserStatus Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public Address Address { get; private set; }

        public string FullName => $"{FirstName} {LastName}";

        public void UpdateProfile(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            
            AddDomainEvent(new UserUpdatedEvent(this));
        }

        public void UpdateAddress(Address address)
        {
            Address = address;
            
            AddDomainEvent(new UserAddressUpdatedEvent(this));
        }

        public void Deactivate()
        {
            Status = UserStatus.Inactive;
            
            AddDomainEvent(new UserDeactivatedEvent(this));
        }

        public void Activate()
        {
            Status = UserStatus.Active;
            
            AddDomainEvent(new UserActivatedEvent(this));
        }
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }
}
