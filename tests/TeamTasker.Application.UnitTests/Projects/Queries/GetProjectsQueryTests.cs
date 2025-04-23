using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Projects.Queries.GetProjects;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.UnitTests.Projects.Queries
{
    public class GetProjectsQueryTests
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly GetProjectsQueryHandler _handler;

        public GetProjectsQueryTests()
        {
            _projectRepository = Substitute.For<IProjectRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _handler = new GetProjectsQueryHandler(_projectRepository, _currentUserService);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllProjects()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project("Project 1", "Description 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(10)) { Id = 1 },
                new Project("Project 2", "Description 2", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(15)) { Id = 2 },
                new Project("Project 3", "Description 3", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(20)) { Id = 3 }
            };

            _projectRepository.ListAllAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Project>>(projects));

            var query = new GetProjectsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            result[0].Id.Should().Be(1);
            result[0].Name.Should().Be("Project 1");
            result[0].Description.Should().Be("Description 1");

            result[1].Id.Should().Be(2);
            result[1].Name.Should().Be("Project 2");
            result[1].Description.Should().Be("Description 2");

            result[2].Id.Should().Be(3);
            result[2].Name.Should().Be("Project 3");
            result[2].Description.Should().Be("Description 3");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProjectsExist()
        {
            // Arrange
            _projectRepository.ListAllAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<Project>>(new List<Project>()));

            var query = new GetProjectsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
