using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Projects.Queries.GetProjectById;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.UnitTests.Projects.Queries
{
    public class GetProjectByIdQueryTests
    {
        private readonly IProjectRepository _projectRepository;
        private readonly GetProjectByIdQueryHandler _handler;

        public GetProjectByIdQueryTests()
        {
            _projectRepository = Substitute.For<IProjectRepository>();
            _handler = new GetProjectByIdQueryHandler(_projectRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnProjectWithTasks_WhenProjectExists()
        {
            // Arrange
            var projectId = 1;
            var project = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(30))
            {
                Id = projectId
            };

            var task1 = project.AddTask("Task 1", "Task 1 Description", DateTime.UtcNow.AddDays(5), Domain.Entities.TaskPriority.High);
            task1.Id = 1;

            var task2 = project.AddTask("Task 2", "Task 2 Description", DateTime.UtcNow.AddDays(10), Domain.Entities.TaskPriority.Medium);
            task2.Id = 2;

            var task3 = project.AddTask("Task 3", "Task 3 Description", DateTime.UtcNow.AddDays(15), Domain.Entities.TaskPriority.Low);
            task3.Id = 3;

            _projectRepository.GetProjectWithTasksAsync(projectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(project));

            var query = new GetProjectByIdQuery { Id = projectId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(projectId);
            result.Name.Should().Be("Test Project");
            result.Description.Should().Be("Test Description");

            result.Tasks.Should().HaveCount(3);

            result.Tasks[0].Id.Should().Be(1);
            result.Tasks[0].Title.Should().Be("Task 1");
            result.Tasks[0].Priority.Should().Be(Domain.Entities.TaskPriority.High.ToString());

            result.Tasks[1].Id.Should().Be(2);
            result.Tasks[1].Title.Should().Be("Task 2");
            result.Tasks[1].Priority.Should().Be(Domain.Entities.TaskPriority.Medium.ToString());

            result.Tasks[2].Id.Should().Be(3);
            result.Tasks[2].Title.Should().Be("Task 3");
            result.Tasks[2].Priority.Should().Be(Domain.Entities.TaskPriority.Low.ToString());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = 999;

            _projectRepository.GetProjectWithTasksAsync(projectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Project>(null));

            var query = new GetProjectByIdQuery { Id = projectId };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }
    }
}
