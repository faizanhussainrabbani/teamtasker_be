using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Projects.Commands.DeleteProject;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.UnitTests.Projects.Commands
{
    public class DeleteProjectCommandTests
    {
        private readonly IProjectRepository _projectRepository;
        private readonly DeleteProjectCommandHandler _handler;

        public DeleteProjectCommandTests()
        {
            _projectRepository = Substitute.For<IProjectRepository>();
            _handler = new DeleteProjectCommandHandler(_projectRepository);
        }

        [Fact]
        public async Task Handle_ShouldDeleteProject_WhenProjectExists()
        {
            // Arrange
            var projectId = 1;
            var existingProject = new Project("Test Project", "Test Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(10))
            {
                Id = projectId
            };

            var command = new DeleteProjectCommand
            {
                Id = projectId
            };

            _projectRepository.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(existingProject));

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            await _projectRepository.Received(1).GetByIdAsync(projectId, Arg.Any<CancellationToken>());
            await _projectRepository.Received(1).DeleteAsync(existingProject, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = 999;
            var command = new DeleteProjectCommand
            {
                Id = projectId
            };

            _projectRepository.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Project>(null));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(command, CancellationToken.None));

            await _projectRepository.Received(1).GetByIdAsync(projectId, Arg.Any<CancellationToken>());
            await _projectRepository.DidNotReceive().DeleteAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
        }
    }
}
