using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
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
        private readonly IApplicationDbContext _dbContext;
        private readonly GetProjectsQueryHandler _handler;

        public GetProjectsQueryTests()
        {
            _projectRepository = Substitute.For<IProjectRepository>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _handler = new GetProjectsQueryHandler(_projectRepository, _currentUserService, _dbContext);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllProjects()
        {
            // Arrange
            var projectDtos = new List<ProjectDto>
            {
                new ProjectDto { Id = 1, Name = "Project 1", Description = "Description 1", Status = "Active" },
                new ProjectDto { Id = 2, Name = "Project 2", Description = "Description 2", Status = "Active" },
                new ProjectDto { Id = 3, Name = "Project 3", Description = "Description 3", Status = "Active" }
            };

            // Mock the DbSet
            var mockDbSet = Substitute.For<DbSet<Project>, IQueryable<Project>>();
            var queryableProjects = new List<Project>
            {
                new Project("Project 1", "Description 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(10)) { Id = 1 },
                new Project("Project 2", "Description 2", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(15)) { Id = 2 },
                new Project("Project 3", "Description 3", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(20)) { Id = 3 }
            }.AsQueryable();

            ((IQueryable<Project>)mockDbSet).Provider.Returns(queryableProjects.Provider);
            ((IQueryable<Project>)mockDbSet).Expression.Returns(queryableProjects.Expression);
            ((IQueryable<Project>)mockDbSet).ElementType.Returns(queryableProjects.ElementType);
            ((IQueryable<Project>)mockDbSet).GetEnumerator().Returns(queryableProjects.GetEnumerator());

            _dbContext.Projects.Returns(mockDbSet);

            var query = new GetProjectsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Count.Should().Be(3);

            result.Items[0].Id.Should().Be(1);
            result.Items[0].Name.Should().Be("Project 1");
            result.Items[0].Description.Should().Be("Description 1");

            result.Items[1].Id.Should().Be(2);
            result.Items[1].Name.Should().Be("Project 2");
            result.Items[1].Description.Should().Be("Description 2");

            result.Items[2].Id.Should().Be(3);
            result.Items[2].Name.Should().Be("Project 3");
            result.Items[2].Description.Should().Be("Description 3");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProjectsExist()
        {
            // Arrange
            // Mock the DbSet with empty list
            var mockDbSet = Substitute.For<DbSet<Project>, IQueryable<Project>>();
            var queryableProjects = new List<Project>().AsQueryable();

            ((IQueryable<Project>)mockDbSet).Provider.Returns(queryableProjects.Provider);
            ((IQueryable<Project>)mockDbSet).Expression.Returns(queryableProjects.Expression);
            ((IQueryable<Project>)mockDbSet).ElementType.Returns(queryableProjects.ElementType);
            ((IQueryable<Project>)mockDbSet).GetEnumerator().Returns(queryableProjects.GetEnumerator());

            _dbContext.Projects.Returns(mockDbSet);

            var query = new GetProjectsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }
    }
}
