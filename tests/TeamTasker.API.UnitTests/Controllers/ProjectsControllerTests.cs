using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TeamTasker.API.Controllers;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Models;
using TeamTasker.Application.Projects.Commands.CreateProject;
using TeamTasker.Application.Projects.Commands.DeleteProject;
using TeamTasker.Application.Projects.Commands.UpdateProject;
using TeamTasker.Application.Projects.Queries.GetProjectById;
using TeamTasker.Application.Projects.Queries.GetProjects;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.API.UnitTests.Controllers
{
    public class ProjectsControllerTests
    {
        private readonly ISender _mediator;
        private readonly ProjectsController _controller;

        public ProjectsControllerTests()
        {
            _mediator = Substitute.For<ISender>();
            _controller = new ProjectsController();

            // Use reflection to set the private _mediator field
            var mediatorField = typeof(ProjectsController).BaseType.GetField("_mediator",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            mediatorField.SetValue(_controller, _mediator);
        }

        [Fact]
        public async Task GetProjects_ShouldReturnOkResult_WithPaginatedListOfProjects()
        {
            // Arrange
            var projects = new List<ProjectDto>
            {
                new ProjectDto { Id = 1, Name = "Project 1", Description = "Description 1" },
                new ProjectDto { Id = 2, Name = "Project 2", Description = "Description 2" }
            };

            var paginatedList = new PaginatedList<ProjectDto>(projects, 2, 1, 10);

            _mediator.Send(Arg.Any<GetProjectsQuery>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(paginatedList));

            // Act
            var result = await _controller.GetProjects();

            // Assert
            var okResult = result.Result.Should().BeOfType<ActionResult<PaginatedList<ProjectDto>>>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<PaginatedList<ProjectDto>>().Subject;

            returnValue.Items.Count.Should().Be(2);
            returnValue.Items[0].Id.Should().Be(1);
            returnValue.Items[0].Name.Should().Be("Project 1");
            returnValue.Items[1].Id.Should().Be(2);
            returnValue.Items[1].Name.Should().Be("Project 2");
            returnValue.TotalCount.Should().Be(2);
            returnValue.TotalPages.Should().Be(1);
            returnValue.PageNumber.Should().Be(1);
            returnValue.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task GetProject_ShouldReturnOkResult_WithProjectDetails_WhenProjectExists()
        {
            // Arrange
            var projectId = 1;
            var projectDetail = new ProjectDetailDto
            {
                Id = projectId,
                Name = "Test Project",
                Description = "Test Description",
                Tasks = new List<TaskDto>
                {
                    new TaskDto { Id = 1, Title = "Task 1" },
                    new TaskDto { Id = 2, Title = "Task 2" }
                }
            };

            _mediator.Send(Arg.Is<GetProjectByIdQuery>(q => q.Id == projectId), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(projectDetail));

            // Act
            var result = await _controller.GetProject(projectId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeAssignableTo<ProjectDetailDto>().Subject;

            returnValue.Id.Should().Be(projectId);
            returnValue.Name.Should().Be("Test Project");
            returnValue.Tasks.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProject_ShouldReturnNotFound_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = 999;

            _mediator.Send(Arg.Is<GetProjectByIdQuery>(q => q.Id == projectId), Arg.Any<CancellationToken>())
                .Returns(Task.FromException<ProjectDetailDto>(new NotFoundException("Project", projectId)));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetProject(projectId));
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WithProjectId()
        {
            // Arrange
            var command = new CreateProjectCommand
            {
                Name = "New Project",
                Description = "New Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            var projectId = 1;
            _mediator.Send(command, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(projectId));

            // Act
            var result = await _controller.Create(command);

            // Assert
            var createdAtActionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtActionResult.ActionName.Should().Be(nameof(ProjectsController.GetProject));
            createdAtActionResult.RouteValues["id"].Should().Be(projectId);
            createdAtActionResult.Value.Should().Be(projectId);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenUpdateSucceeds()
        {
            // Arrange
            var projectId = 1;
            var command = new UpdateProjectCommand
            {
                Id = projectId,
                Name = "Updated Project",
                Description = "Updated Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _mediator.Send(command, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(projectId, command);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdsMismatch()
        {
            // Arrange
            var projectId = 1;
            var command = new UpdateProjectCommand
            {
                Id = 2, // Different ID
                Name = "Updated Project",
                Description = "Updated Description"
            };

            // Act
            var result = await _controller.Update(projectId, command);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenDeleteSucceeds()
        {
            // Arrange
            var projectId = 1;

            _mediator.Send(Arg.Is<DeleteProjectCommand>(c => c.Id == projectId), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(projectId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
