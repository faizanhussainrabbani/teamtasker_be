using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TeamTasker.Domain.Entities;
using TeamTasker.Infrastructure.Data;
using TeamTasker.Infrastructure.Repositories;
using TeamTasker.SharedKernel.Interfaces;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Infrastructure.UnitTests.Repositories
{
    public class TaskRepositoryTests
    {
        [Fact]
        public async Task GetTasksByUserIdAsync_ShouldReturnTasksAssignedToUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            var userId = 5;

            // Create and seed the database
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

                var task1 = project.AddTask("Task 1", "Task 1 Description", DateTime.UtcNow.AddDays(5), TaskPriority.High);
                task1.AssignToUser(userId);

                var task2 = project.AddTask("Task 2", "Task 2 Description", DateTime.UtcNow.AddDays(10), TaskPriority.Medium);
                task2.AssignToUser(userId);

                var task3 = project.AddTask("Task 3", "Task 3 Description", DateTime.UtcNow.AddDays(15), TaskPriority.Low);
                task3.AssignToUser(999); // Different user

                context.Projects.Add(project);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new TaskRepository(context);

                // Act
                var result = await repository.GetTasksByUserIdAsync(userId);

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result.Select(t => t.Title).Should().BeEquivalentTo("Task 1", "Task 2");
            }
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_ShouldReturnTasksForProject()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();

            // Create and seed the database
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var project1 = new Project("Project 1", "Description 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
                project1.AddTask("Task 1", "Task 1 Description", DateTime.UtcNow.AddDays(5), TaskPriority.High);
                project1.AddTask("Task 2", "Task 2 Description", DateTime.UtcNow.AddDays(10), TaskPriority.Medium);

                var project2 = new Project("Project 2", "Description 2", DateTime.UtcNow, DateTime.UtcNow.AddDays(20));
                project2.AddTask("Task 3", "Task 3 Description", DateTime.UtcNow.AddDays(15), TaskPriority.Low);

                context.Projects.AddRange(project1, project2);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new TaskRepository(context);

                // Act
                var result = await repository.GetTasksByProjectIdAsync(1); // Project 1

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result.Select(t => t.Title).Should().BeEquivalentTo("Task 1", "Task 2");
            }
        }
    }
}
