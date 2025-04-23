using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TeamTasker.Domain.Entities;
using TeamTasker.Infrastructure.Data;
using TeamTasker.SharedKernel.Interfaces;
using Xunit;

namespace TeamTasker.Infrastructure.UnitTests.Data
{
    public class EfRepositoryTests
    {
        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            
            // Create and seed the database
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
                context.Projects.Add(project);
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new EfRepository<Project>(context);
                
                // Act
                var result = await repository.GetByIdAsync(1);

                // Assert
                result.Should().NotBeNull();
                result.Name.Should().Be("Test Project");
                result.Description.Should().Be("Test Description");
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new EfRepository<Project>(context);
                
                // Act
                var result = await repository.GetByIdAsync(999);

                // Assert
                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task ListAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            
            // Create and seed the database
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                context.Projects.Add(new Project("Project 1", "Description 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(10)));
                context.Projects.Add(new Project("Project 2", "Description 2", DateTime.UtcNow, DateTime.UtcNow.AddDays(20)));
                context.Projects.Add(new Project("Project 3", "Description 3", DateTime.UtcNow, DateTime.UtcNow.AddDays(30)));
                await context.SaveChangesAsync();
            }

            // Use a separate instance of the context to verify correct data was saved
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new EfRepository<Project>(context);
                
                // Act
                var result = await repository.ListAllAsync();

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(3);
                result.Select(p => p.Name).Should().BeEquivalentTo("Project 1", "Project 2", "Project 3");
            }
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntityToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
            
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new EfRepository<Project>(context);
                
                // Act
                await repository.AddAsync(project);
            }

            // Use a separate instance of the context to verify correct data was saved
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                // Assert
                context.Projects.Should().HaveCount(1);
                context.Projects.First().Name.Should().Be("Test Project");
                context.Projects.First().Description.Should().Be("Test Description");
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntityInDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            var project = new Project("Old Name", "Old Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
            
            // Create and seed the database
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                context.Projects.Add(project);
                await context.SaveChangesAsync();
            }

            // Update the entity
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new EfRepository<Project>(context);
                var projectToUpdate = await context.Projects.FirstAsync();
                projectToUpdate.UpdateDetails("New Name", "New Description", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(30));
                
                // Act
                await repository.UpdateAsync(projectToUpdate);
            }

            // Use a separate instance of the context to verify correct data was saved
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                // Assert
                context.Projects.Should().HaveCount(1);
                context.Projects.First().Name.Should().Be("New Name");
                context.Projects.First().Description.Should().Be("New Description");
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntityFromDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            var domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(30));
            
            // Create and seed the database
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                context.Projects.Add(project);
                await context.SaveChangesAsync();
            }

            // Delete the entity
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                var repository = new EfRepository<Project>(context);
                var projectToDelete = await context.Projects.FirstAsync();
                
                // Act
                await repository.DeleteAsync(projectToDelete);
            }

            // Use a separate instance of the context to verify entity was deleted
            using (var context = new ApplicationDbContext(options, domainEventDispatcher))
            {
                // Assert
                context.Projects.Should().BeEmpty();
            }
        }
    }
}
