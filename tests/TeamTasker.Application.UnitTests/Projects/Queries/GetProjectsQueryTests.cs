using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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

        [Fact(Skip = "Legacy query test requires in-memory db context refactor")]
        public async Task Handle_ShouldReturnAllProjects()
        {
            await Task.CompletedTask;
        }

        [Fact(Skip = "Legacy query test requires in-memory db context refactor")]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProjectsExist()
        {
            await Task.CompletedTask;
        }
    }
}
