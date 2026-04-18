using System;
using System.Threading;
using FluentAssertions;
using NSubstitute;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Projects.Commands.UpdateProjectStatus;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.UnitTests.Projects.Commands
{
    public class UpdateProjectStatusCommandTests
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UpdateProjectStatusCommandHandler _handler;

        public UpdateProjectStatusCommandTests()
        {
            _projectRepository = Substitute.For<IProjectRepository>();
            _handler = new UpdateProjectStatusCommandHandler(_projectRepository);
        }

        [Fact]
        public async Task Handle_ShouldUpdateProjectStatus_WhenStatusIsValid()
        {
            var project = new Project(
                "Migration",
                "Upgrade legacy modules",
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(30))
            {
                Id = 7
            };

            _projectRepository.GetByIdAsync(7, Arg.Any<CancellationToken>())
                .Returns(project);

            var result = await _handler.Handle(new UpdateProjectStatusCommand
            {
                Id = 7,
                Status = "in-progress"
            }, CancellationToken.None);

            result.Id.Should().Be(7);
            result.Status.Should().Be(ProjectStatus.InProgress.ToString());

            await _projectRepository.Received(1)
                .UpdateAsync(Arg.Is<Project>(entity => entity.Status == ProjectStatus.InProgress), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenProjectDoesNotExist()
        {
            _projectRepository.GetByIdAsync(11, Arg.Any<CancellationToken>())
                .Returns((Project)null);

            Func<Task> act = async () => await _handler.Handle(new UpdateProjectStatusCommand
            {
                Id = 11,
                Status = "completed"
            }, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenStatusIsInvalid()
        {
            var project = new Project(
                "Roadmap",
                "Quarter planning",
                DateTime.UtcNow,
                null)
            {
                Id = 15
            };

            _projectRepository.GetByIdAsync(15, Arg.Any<CancellationToken>())
                .Returns(project);

            Func<Task> act = async () => await _handler.Handle(new UpdateProjectStatusCommand
            {
                Id = 15,
                Status = "invalid-status"
            }, CancellationToken.None);

            await act.Should().ThrowAsync<BadRequestException>();
        }
    }
}
