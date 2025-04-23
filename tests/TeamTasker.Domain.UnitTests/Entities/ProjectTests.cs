using System;
using FluentAssertions;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Events;
using Xunit;

namespace TeamTasker.Domain.UnitTests.Entities
{
    public class ProjectTests
    {
        [Fact]
        public void Constructor_ShouldCreateProject_WithCorrectProperties()
        {
            // Arrange
            var name = "Test Project";
            var description = "Test Description";
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(30);

            // Act
            var project = new Project(name, description, startDate, endDate);

            // Assert
            project.Name.Should().Be(name);
            project.Description.Should().Be(description);
            project.StartDate.Should().Be(startDate);
            project.EndDate.Should().Be(endDate);
            project.Status.Should().Be(ProjectStatus.NotStarted);
            project.Tasks.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_ShouldRaiseProjectCreatedEvent()
        {
            // Arrange
            var name = "Test Project";
            var description = "Test Description";
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(30);

            // Act
            var project = new Project(name, description, startDate, endDate);

            // Assert
            project.DomainEvents.Should().ContainSingle();
            project.DomainEvents.Should().ContainItemsAssignableTo<ProjectCreatedEvent>();
            var @event = project.DomainEvents.Should().ContainSingle(e => e is ProjectCreatedEvent).Subject as ProjectCreatedEvent;
            @event.Project.Should().Be(project);
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateProjectProperties()
        {
            // Arrange
            var project = new Project("Old Name", "Old Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
            var newName = "New Name";
            var newDescription = "New Description";
            var newStartDate = DateTime.UtcNow.AddDays(5);
            var newEndDate = newStartDate.AddDays(30);

            // Clear domain events from constructor
            project.ClearDomainEvents();

            // Act
            project.UpdateDetails(newName, newDescription, newStartDate, newEndDate);

            // Assert
            project.Name.Should().Be(newName);
            project.Description.Should().Be(newDescription);
            project.StartDate.Should().Be(newStartDate);
            project.EndDate.Should().Be(newEndDate);
        }

        [Fact]
        public void UpdateDetails_ShouldRaiseProjectUpdatedEvent()
        {
            // Arrange
            var project = new Project("Old Name", "Old Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));

            // Clear domain events from constructor
            project.ClearDomainEvents();

            // Act
            project.UpdateDetails("New Name", "New Description", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(35));

            // Assert
            project.DomainEvents.Should().ContainSingle();
            project.DomainEvents.Should().ContainItemsAssignableTo<ProjectUpdatedEvent>();
            var @event = project.DomainEvents.Should().ContainSingle(e => e is ProjectUpdatedEvent).Subject as ProjectUpdatedEvent;
            @event.Project.Should().Be(project);
        }

        [Fact]
        public void UpdateStatus_ShouldUpdateProjectStatus()
        {
            // Arrange
            var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
            var newStatus = ProjectStatus.InProgress;

            // Clear domain events from constructor
            project.ClearDomainEvents();

            // Act
            project.UpdateStatus(newStatus);

            // Assert
            project.Status.Should().Be(newStatus);
        }

        [Fact]
        public void UpdateStatus_ShouldRaiseProjectStatusUpdatedEvent()
        {
            // Arrange
            var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));

            // Clear domain events from constructor
            project.ClearDomainEvents();

            // Act
            project.UpdateStatus(ProjectStatus.InProgress);

            // Assert
            project.DomainEvents.Should().ContainSingle();
            project.DomainEvents.Should().ContainItemsAssignableTo<ProjectStatusUpdatedEvent>();
            var @event = project.DomainEvents.Should().ContainSingle(e => e is ProjectStatusUpdatedEvent).Subject as ProjectStatusUpdatedEvent;
            @event.Project.Should().Be(project);
        }

        [Fact]
        public void AddTask_ShouldAddTaskToProject()
        {
            // Arrange
            var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
            var taskTitle = "Test Task";
            var taskDescription = "Test Task Description";
            var taskDueDate = DateTime.UtcNow.AddDays(5);
            var taskPriority = TaskPriority.Medium;

            // Clear domain events from constructor
            project.ClearDomainEvents();

            // Act
            var task = project.AddTask(taskTitle, taskDescription, taskDueDate, taskPriority);

            // Assert
            project.Tasks.Should().ContainSingle();
            project.Tasks.Should().Contain(task);
            task.Title.Should().Be(taskTitle);
            task.Description.Should().Be(taskDescription);
            task.DueDate.Should().Be(taskDueDate);
            task.Priority.Should().Be(taskPriority);
            task.ProjectId.Should().Be(project.Id);
        }

        [Fact]
        public void AddTask_ShouldRaiseTaskAddedToProjectEvent()
        {
            // Arrange
            var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));

            // Clear domain events from constructor
            project.ClearDomainEvents();

            // Act
            var task = project.AddTask("Test Task", "Test Description", DateTime.UtcNow.AddDays(5), TaskPriority.Medium);

            // Assert
            project.DomainEvents.Should().ContainSingle();
            project.DomainEvents.Should().ContainItemsAssignableTo<TaskAddedToProjectEvent>();
            var @event = project.DomainEvents.Should().ContainSingle(e => e is TaskAddedToProjectEvent).Subject as TaskAddedToProjectEvent;
            @event.Project.Should().Be(project);
            @event.TaskItem.Should().Be(task);
        }
    }
}
