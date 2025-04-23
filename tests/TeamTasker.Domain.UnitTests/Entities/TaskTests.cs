using System;
using FluentAssertions;
using TeamTasker.Domain.Events;
using Xunit;

namespace TeamTasker.Domain.UnitTests.Entities
{
    public class TaskTests
    {
        [Fact]
        public void Constructor_ShouldCreateTask_WithCorrectProperties()
        {
            // Arrange
            var title = "Test Task";
            var description = "Test Description";
            var dueDate = DateTime.UtcNow.AddDays(5);
            var priority = Domain.Entities.TaskPriority.High;
            var projectId = 1;

            // Act
            var task = new Domain.Entities.Task(title, description, dueDate, priority, projectId);

            // Assert
            task.Title.Should().Be(title);
            task.Description.Should().Be(description);
            task.DueDate.Should().Be(dueDate);
            task.Priority.Should().Be(priority);
            task.Status.Should().Be(Domain.Entities.TaskStatus.ToDo);
            task.ProjectId.Should().Be(projectId);
            task.AssignedToUserId.Should().BeNull();
            task.CompletedDate.Should().BeNull();
            task.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Constructor_ShouldRaiseTaskCreatedEvent()
        {
            // Arrange
            var title = "Test Task";
            var description = "Test Description";
            var dueDate = DateTime.UtcNow.AddDays(5);
            var priority = Domain.Entities.TaskPriority.High;
            var projectId = 1;

            // Act
            var task = new Domain.Entities.Task(title, description, dueDate, priority, projectId);

            // Assert
            task.DomainEvents.Should().ContainSingle();
            task.DomainEvents.Should().ContainItemsAssignableTo<TaskCreatedEvent>();
            var @event = task.DomainEvents.Should().ContainSingle(e => e is TaskCreatedEvent).Subject as TaskCreatedEvent;
            @event.TaskItem.Should().Be(task);
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateTaskProperties()
        {
            // Arrange
            var task = new Domain.Entities.Task("Old Title", "Old Description", DateTime.UtcNow, Domain.Entities.TaskPriority.Low, 1);
            var newTitle = "New Title";
            var newDescription = "New Description";
            var newDueDate = DateTime.UtcNow.AddDays(10);
            var newPriority = Domain.Entities.TaskPriority.Critical;

            // Clear domain events from constructor
            task.ClearDomainEvents();

            // Act
            task.UpdateDetails(newTitle, newDescription, newDueDate, newPriority);

            // Assert
            task.Title.Should().Be(newTitle);
            task.Description.Should().Be(newDescription);
            task.DueDate.Should().Be(newDueDate);
            task.Priority.Should().Be(newPriority);
        }

        [Fact]
        public void UpdateDetails_ShouldRaiseTaskUpdatedEvent()
        {
            // Arrange
            var task = new Domain.Entities.Task("Old Title", "Old Description", DateTime.UtcNow, Domain.Entities.TaskPriority.Low, 1);

            // Clear domain events from constructor
            task.ClearDomainEvents();

            // Act
            task.UpdateDetails("New Title", "New Description", DateTime.UtcNow.AddDays(10), Domain.Entities.TaskPriority.Critical);

            // Assert
            task.DomainEvents.Should().ContainSingle();
            task.DomainEvents.Should().ContainItemsAssignableTo<TaskUpdatedEvent>();
            var @event = task.DomainEvents.Should().ContainSingle(e => e is TaskUpdatedEvent).Subject as TaskUpdatedEvent;
            @event.TaskItem.Should().Be(task);
        }

        [Fact]
        public void UpdateStatus_ShouldUpdateTaskStatus()
        {
            // Arrange
            var task = new Domain.Entities.Task("Test Task", "Test Description", DateTime.UtcNow, Domain.Entities.TaskPriority.Medium, 1);
            var newStatus = Domain.Entities.TaskStatus.InProgress;

            // Clear domain events from constructor
            task.ClearDomainEvents();

            // Act
            task.UpdateStatus(newStatus);

            // Assert
            task.Status.Should().Be(newStatus);
            task.CompletedDate.Should().BeNull();
        }

        [Fact]
        public void UpdateStatus_WhenStatusIsDone_ShouldSetCompletedDate()
        {
            // Arrange
            var task = new Domain.Entities.Task("Test Task", "Test Description", DateTime.UtcNow, Domain.Entities.TaskPriority.Medium, 1);

            // Clear domain events from constructor
            task.ClearDomainEvents();

            // Act
            task.UpdateStatus(Domain.Entities.TaskStatus.Done);

            // Assert
            task.Status.Should().Be(Domain.Entities.TaskStatus.Done);
            task.CompletedDate.Should().NotBeNull();
            task.CompletedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void UpdateStatus_ShouldRaiseTaskStatusUpdatedEvent()
        {
            // Arrange
            var task = new Domain.Entities.Task("Test Task", "Test Description", DateTime.UtcNow, Domain.Entities.TaskPriority.Medium, 1);

            // Clear domain events from constructor
            task.ClearDomainEvents();

            // Act
            task.UpdateStatus(Domain.Entities.TaskStatus.InProgress);

            // Assert
            task.DomainEvents.Should().ContainSingle();
            task.DomainEvents.Should().ContainItemsAssignableTo<TaskStatusUpdatedEvent>();
            var @event = task.DomainEvents.Should().ContainSingle(e => e is TaskStatusUpdatedEvent).Subject as TaskStatusUpdatedEvent;
            @event.TaskItem.Should().Be(task);
        }

        [Fact]
        public void AssignToUser_ShouldSetAssignedToUserId()
        {
            // Arrange
            var task = new Domain.Entities.Task("Test Task", "Test Description", DateTime.UtcNow, Domain.Entities.TaskPriority.Medium, 1);
            var userId = 5;

            // Clear domain events from constructor
            task.ClearDomainEvents();

            // Act
            task.AssignToUser(userId);

            // Assert
            task.AssignedToUserId.Should().Be(userId);
        }

        [Fact]
        public void AssignToUser_ShouldRaiseTaskAssignedEvent()
        {
            // Arrange
            var task = new Domain.Entities.Task("Test Task", "Test Description", DateTime.UtcNow, Domain.Entities.TaskPriority.Medium, 1);
            var userId = 5;

            // Clear domain events from constructor
            task.ClearDomainEvents();

            // Act
            task.AssignToUser(userId);

            // Assert
            task.DomainEvents.Should().ContainSingle();
            task.DomainEvents.Should().ContainItemsAssignableTo<TaskAssignedEvent>();
            var @event = task.DomainEvents.Should().ContainSingle(e => e is TaskAssignedEvent).Subject as TaskAssignedEvent;
            @event.TaskItem.Should().Be(task);
            @event.UserId.Should().Be(userId);
        }

        [Fact]
        public void RemoveAssignment_ShouldClearAssignedToUserId()
        {
            // Arrange
            var task = new Domain.Entities.Task("Test Task", "Test Description", DateTime.UtcNow, Domain.Entities.TaskPriority.Medium, 1);
            task.AssignToUser(5);

            // Clear domain events from constructor and assignment
            task.ClearDomainEvents();

            // Act
            task.RemoveAssignment();

            // Assert
            task.AssignedToUserId.Should().BeNull();
        }

        [Fact]
        public void RemoveAssignment_ShouldRaiseTaskUnassignedEvent()
        {
            // Arrange
            var task = new Domain.Entities.Task("Test Task", "Test Description", DateTime.UtcNow, Domain.Entities.TaskPriority.Medium, 1);
            task.AssignToUser(5);

            // Clear domain events from constructor and assignment
            task.ClearDomainEvents();

            // Act
            task.RemoveAssignment();

            // Assert
            task.DomainEvents.Should().ContainSingle();
            task.DomainEvents.Should().ContainItemsAssignableTo<TaskUnassignedEvent>();
            var @event = task.DomainEvents.Should().ContainSingle(e => e is TaskUnassignedEvent).Subject as TaskUnassignedEvent;
            @event.TaskItem.Should().Be(task);
        }
    }
}
