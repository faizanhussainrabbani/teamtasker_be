using System;
using FluentAssertions;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Events;
using TeamTasker.Domain.ValueObjects;
using Xunit;

namespace TeamTasker.Domain.UnitTests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Constructor_ShouldCreateUser_WithCorrectProperties()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@example.com";
            var username = "johndoe";

            // Act
            var user = new User(firstName, lastName, email, username);

            // Assert
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Email.Should().Be(email);
            user.Username.Should().Be(username);
            user.Status.Should().Be(UserStatus.Active);
            user.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            user.Address.Should().BeNull();
            user.FullName.Should().Be($"{firstName} {lastName}");
        }

        [Fact]
        public void Constructor_ShouldRaiseUserCreatedEvent()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@example.com";
            var username = "johndoe";

            // Act
            var user = new User(firstName, lastName, email, username);

            // Assert
            user.DomainEvents.Should().ContainSingle();
            user.DomainEvents.Should().ContainItemsAssignableTo<UserCreatedEvent>();
            var @event = user.DomainEvents.Should().ContainSingle(e => e is UserCreatedEvent).Subject as UserCreatedEvent;
            @event.User.Should().Be(user);
        }

        [Fact]
        public void UpdateProfile_ShouldUpdateUserProperties()
        {
            // Arrange
            var user = new User("Old First", "Old Last", "old.email@example.com", "oldusername");
            var newFirstName = "New First";
            var newLastName = "New Last";
            var newEmail = "new.email@example.com";
            
            // Clear domain events from constructor
            user.ClearDomainEvents();

            // Act
            user.UpdateProfile(newFirstName, newLastName, newEmail);

            // Assert
            user.FirstName.Should().Be(newFirstName);
            user.LastName.Should().Be(newLastName);
            user.Email.Should().Be(newEmail);
            user.FullName.Should().Be($"{newFirstName} {newLastName}");
        }

        [Fact]
        public void UpdateProfile_ShouldRaiseUserUpdatedEvent()
        {
            // Arrange
            var user = new User("Old First", "Old Last", "old.email@example.com", "oldusername");
            
            // Clear domain events from constructor
            user.ClearDomainEvents();

            // Act
            user.UpdateProfile("New First", "New Last", "new.email@example.com");

            // Assert
            user.DomainEvents.Should().ContainSingle();
            user.DomainEvents.Should().ContainItemsAssignableTo<UserUpdatedEvent>();
            var @event = user.DomainEvents.Should().ContainSingle(e => e is UserUpdatedEvent).Subject as UserUpdatedEvent;
            @event.User.Should().Be(user);
        }

        [Fact]
        public void UpdateAddress_ShouldSetUserAddress()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", "johndoe");
            var address = new Address("123 Main St", "New York", "NY", "USA", "10001");
            
            // Clear domain events from constructor
            user.ClearDomainEvents();

            // Act
            user.UpdateAddress(address);

            // Assert
            user.Address.Should().Be(address);
        }

        [Fact]
        public void UpdateAddress_ShouldRaiseUserAddressUpdatedEvent()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", "johndoe");
            var address = new Address("123 Main St", "New York", "NY", "USA", "10001");
            
            // Clear domain events from constructor
            user.ClearDomainEvents();

            // Act
            user.UpdateAddress(address);

            // Assert
            user.DomainEvents.Should().ContainSingle();
            user.DomainEvents.Should().ContainItemsAssignableTo<UserAddressUpdatedEvent>();
            var @event = user.DomainEvents.Should().ContainSingle(e => e is UserAddressUpdatedEvent).Subject as UserAddressUpdatedEvent;
            @event.User.Should().Be(user);
        }

        [Fact]
        public void Deactivate_ShouldSetUserStatusToInactive()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", "johndoe");
            
            // Clear domain events from constructor
            user.ClearDomainEvents();

            // Act
            user.Deactivate();

            // Assert
            user.Status.Should().Be(UserStatus.Inactive);
        }

        [Fact]
        public void Deactivate_ShouldRaiseUserDeactivatedEvent()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", "johndoe");
            
            // Clear domain events from constructor
            user.ClearDomainEvents();

            // Act
            user.Deactivate();

            // Assert
            user.DomainEvents.Should().ContainSingle();
            user.DomainEvents.Should().ContainItemsAssignableTo<UserDeactivatedEvent>();
            var @event = user.DomainEvents.Should().ContainSingle(e => e is UserDeactivatedEvent).Subject as UserDeactivatedEvent;
            @event.User.Should().Be(user);
        }

        [Fact]
        public void Activate_ShouldSetUserStatusToActive()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", "johndoe");
            user.Deactivate();
            
            // Clear domain events from constructor and deactivation
            user.ClearDomainEvents();

            // Act
            user.Activate();

            // Assert
            user.Status.Should().Be(UserStatus.Active);
        }

        [Fact]
        public void Activate_ShouldRaiseUserActivatedEvent()
        {
            // Arrange
            var user = new User("John", "Doe", "john.doe@example.com", "johndoe");
            user.Deactivate();
            
            // Clear domain events from constructor and deactivation
            user.ClearDomainEvents();

            // Act
            user.Activate();

            // Assert
            user.DomainEvents.Should().ContainSingle();
            user.DomainEvents.Should().ContainItemsAssignableTo<UserActivatedEvent>();
            var @event = user.DomainEvents.Should().ContainSingle(e => e is UserActivatedEvent).Subject as UserActivatedEvent;
            @event.User.Should().Be(user);
        }
    }
}
