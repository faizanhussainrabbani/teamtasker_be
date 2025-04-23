using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Projects.Commands.UpdateProject;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.UnitTests.Projects.Commands
{
    public class UpdateProjectCommandTests
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UpdateProjectCommandHandler _handler;

        public UpdateProjectCommandTests()
        {
            _projectRepository = Substitute.For<IProjectRepository>();
            _handler = new UpdateProjectCommandHandler(_projectRepository);
        }

        [Fact]
        public async Task Handle_ShouldUpdateProject_WhenProjectExists()
        {
            // Arrange
            var projectId = 1;
            var existingProject = new Project("Old Name", "Old Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(10))
            {
                Id = projectId
            };

            var command = new UpdateProjectCommand
            {
                Id = projectId,
                Name = "New Name",
                Description = "New Description",
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _projectRepository.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(existingProject));

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            await _projectRepository.Received(1).GetByIdAsync(projectId, Arg.Any<CancellationToken>());
            await _projectRepository.Received(1).UpdateAsync(Arg.Is<Project>(p =>
                p.Id == projectId &&
                p.Name == command.Name &&
                p.Description == command.Description &&
                p.StartDate == command.StartDate &&
                p.EndDate == command.EndDate),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = 999;
            var command = new UpdateProjectCommand
            {
                Id = projectId,
                Name = "New Name",
                Description = "New Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _projectRepository.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Project>(null));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));

            await _projectRepository.Received(1).GetByIdAsync(projectId, Arg.Any<CancellationToken>());
            await _projectRepository.DidNotReceive().UpdateAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
        }
    }
}
