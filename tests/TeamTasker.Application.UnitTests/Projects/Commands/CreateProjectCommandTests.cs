using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using TeamTasker.Application.Projects.Commands.CreateProject;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.UnitTests.Projects.Commands
{
    public class CreateProjectCommandTests
    {
        private readonly IProjectRepository _projectRepository;
        private readonly CreateProjectCommandHandler _handler;

        public CreateProjectCommandTests()
        {
            _projectRepository = Substitute.For<IProjectRepository>();
            _handler = new CreateProjectCommandHandler(_projectRepository);
        }

        [Fact]
        public async Task Handle_ShouldCreateProject_WithCorrectValues()
        {
            // Arrange
            var command = new CreateProjectCommand
            {
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            Project capturedProject = null;
            _projectRepository.AddAsync(Arg.Do<Project>(p => capturedProject = p), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new Project(command.Name, command.Description, command.StartDate, command.EndDate) { Id = 1 }));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(1);

            await _projectRepository.Received(1).AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());

            capturedProject.Should().NotBeNull();
            capturedProject.Name.Should().Be(command.Name);
            capturedProject.Description.Should().Be(command.Description);
            capturedProject.StartDate.Should().Be(command.StartDate);
            capturedProject.EndDate.Should().Be(command.EndDate);
            capturedProject.Status.Should().Be(ProjectStatus.NotStarted);
        }
    }
}
