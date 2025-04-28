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
    public class ProjectRepositoryTests
    {
        [Fact]
        public async Task GetProjectWithTasksAsync_ShouldReturnProjectWithTasks_WhenProjectExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<ProjectRepository>>();

            // Create and seed the database
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
                project.AddTask("Task 1", "Task 1 Description", DateTime.UtcNow.AddDays(5), TaskPriority.High);
                project.AddTask("Task 2", "Task 2 Description", DateTime.UtcNow.AddDays(10), TaskPriority.Medium);
                project.AddTask("Task 3", "Task 3 Description", DateTime.UtcNow.AddDays(15), TaskPriority.Low);

                context.Projects.Add(project);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new ProjectRepository(context, logger);

                // Act
                var result = await repository.GetProjectWithTasksAsync(1);

                // Assert
                result.Should().NotBeNull();
                result.Name.Should().Be("Test Project");
                result.Description.Should().Be("Test Description");

                result.Tasks.Should().HaveCount(3);
                result.Tasks.Select(t => t.Title).Should().BeEquivalentTo("Task 1", "Task 2", "Task 3");
            }
        }

        [Fact]
        public async Task GetProjectWithTasksAsync_ShouldReturnNull_WhenProjectDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<ProjectRepository>>();

            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new ProjectRepository(context, logger);

                // Act
                var result = await repository.GetProjectWithTasksAsync(999);

                // Assert
                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetProjectsByUserIdAsync_ShouldReturnProjectsAssignedToUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<ProjectRepository>>();
            var userId = 5;
            var teamMemberId1 = 10;
            var teamMemberId2 = 11;
            var teamMemberId3 = 12;

            // Create and seed the database
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                // Create team members for the user
                var user = new User("Test", "User", "test@example.com", "testuser", "password", "User");
                user.Id = userId;
                context.Users.Add(user);

                var teamMember1 = new TeamMember(1, userId, "Developer");
                teamMember1.Id = teamMemberId1;

                var teamMember2 = new TeamMember(1, userId, "Developer");
                teamMember2.Id = teamMemberId2;

                var teamMember3 = new TeamMember(1, 999, "Developer"); // Different user
                teamMember3.Id = teamMemberId3;

                context.TeamMembers.AddRange(teamMember1, teamMember2, teamMember3);
                await context.SaveChangesAsync();

                // Project 1 with tasks assigned to user's team member
                var project1 = new Project("Project 1", "Description 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
                var task1 = project1.AddTask("Task 1", "Task 1 Description", DateTime.UtcNow.AddDays(5), TaskPriority.High);
                task1.AssignToTeamMember(teamMemberId1);

                // Project 2 with tasks assigned to user's team member
                var project2 = new Project("Project 2", "Description 2", DateTime.UtcNow, DateTime.UtcNow.AddDays(20));
                var task2 = project2.AddTask("Task 2", "Task 2 Description", DateTime.UtcNow.AddDays(10), TaskPriority.Medium);
                task2.AssignToTeamMember(teamMemberId2);

                // Project 3 with no tasks assigned to user's team member
                var project3 = new Project("Project 3", "Description 3", DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
                var task3 = project3.AddTask("Task 3", "Task 3 Description", DateTime.UtcNow.AddDays(15), TaskPriority.Low);
                task3.AssignToTeamMember(teamMemberId3); // Different user's team member

                context.Projects.AddRange(project1, project2, project3);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new ProjectRepository(context, logger);

                // Act
                var result = await repository.GetProjectsByUserIdAsync(userId);

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2);
                result.Select(p => p.Name).Should().BeEquivalentTo("Project 1", "Project 2");
            }
        }
    }
}
