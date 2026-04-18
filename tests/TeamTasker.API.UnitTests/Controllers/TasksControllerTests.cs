using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using MediatR;
using NSubstitute;
using TeamTasker.API.Controllers;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Models;
using TeamTasker.Application.Tasks.Commands.CreateTask;
using TeamTasker.Application.Tasks.Commands.DeleteTask;
using TeamTasker.Application.Tasks.Commands.UpdateTask;
using TeamTasker.Application.Tasks.Commands.UpdateTaskProgress;
using TeamTasker.Application.Tasks.Commands.UpdateTaskStatus;
using TeamTasker.Application.Tasks.Common;
using TeamTasker.Application.Tasks.Queries.GetTaskById;
using TeamTasker.Application.Tasks.Queries.GetTasks;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.API.UnitTests.Controllers
{
    public class TasksControllerTests
    {
        private readonly ISender _mediator;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _mediator = Substitute.For<ISender>();
            _controller = new TasksController();

            var mediatorField = typeof(TasksController).BaseType?.GetField(
                "_mediator",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            mediatorField?.SetValue(_controller, _mediator);
        }

        [Fact]
        public async Task GetTasks_ShouldReturnPaginatedTasks()
        {
            var paginated = new PaginatedList<TaskDto>(
                new List<TaskDto>
                {
                    new TaskDto { Id = "1", Title = "Task 1", Status = "todo", Priority = "high" }
                },
                1,
                1,
                10);

            _mediator.Send(Arg.Any<GetTasksQuery>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(paginated));

            var result = await _controller.GetTasks(new GetTasksQuery());

            result.Value.Should().NotBeNull();
            result.Value.Items.Should().HaveCount(1);
            result.Value.Items[0].Title.Should().Be("Task 1");
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenBodyIdDoesNotMatchRouteId()
        {
            var command = new UpdateTaskCommand
            {
                Id = 2,
                Title = "Updated"
            };

            var result = await _controller.Update(1, command);

            var badRequest = result.Result.Should()
                .BeOfType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>()
                .Subject;

            badRequest.Value.Should().NotBeNull();

            var statusValue = badRequest.Value.GetType().GetProperty("status")?.GetValue(badRequest.Value);
            var titleValue = badRequest.Value.GetType().GetProperty("title")?.GetValue(badRequest.Value);
            var detailValue = badRequest.Value.GetType().GetProperty("detail")?.GetValue(badRequest.Value);

            statusValue.Should().Be(400);
            titleValue.Should().Be("Bad Request");
            detailValue.Should().Be("Task id in route and body must match.");
        }

        [Fact]
        public async Task GetTask_ShouldReturnTask_WhenMediatorReturnsTask()
        {
            var expectedTask = new TaskDto { Id = "5", Title = "Task detail" };

            _mediator.Send(Arg.Any<GetTaskByIdQuery>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(expectedTask));

            var result = await _controller.GetTask(5);

            result.Value.Should().NotBeNull();
            result.Value.Title.Should().Be("Task detail");
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenMediatorReturnsTask()
        {
            var createdTask = new TaskDto { Id = "7", Title = "Created" };

            _mediator.Send(Arg.Any<CreateTaskCommand>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(createdTask));

            var result = await _controller.Create(new CreateTaskCommand { Title = "Created" });

            var createdAt = result.Result.Should().BeOfType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>().Subject;
            createdAt.ActionName.Should().Be(nameof(TasksController.GetTask));
            createdAt.Value.Should().BeEquivalentTo(createdTask);
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnOk_WhenMediatorReturnsTask()
        {
            var updatedTask = new TaskDto { Id = "1", Status = "completed" };

            _mediator.Send(Arg.Any<UpdateTaskStatusCommand>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(updatedTask));

            var result = await _controller.UpdateStatus(1, new UpdateTaskStatusRequest { Status = "completed" });

            result.Result.Should().BeOfType<Microsoft.AspNetCore.Mvc.OkObjectResult>();
        }

        [Fact]
        public async Task UpdateProgress_ShouldReturnOk_WhenMediatorReturnsTask()
        {
            var updatedTask = new TaskDto { Id = "1", Progress = 80 };

            _mediator.Send(Arg.Any<UpdateTaskProgressCommand>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(updatedTask));

            var result = await _controller.UpdateProgress(1, new UpdateTaskProgressRequest { Progress = 80 });

            result.Result.Should().BeOfType<Microsoft.AspNetCore.Mvc.OkObjectResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenMediatorCompletes()
        {
            _mediator.Send(Arg.Any<DeleteTaskCommand>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.NoContentResult>();
        }

        [Fact]
        public async Task GetTask_ShouldThrowNotFound_WhenMediatorThrowsNotFound()
        {
            _mediator.Send(Arg.Any<GetTaskByIdQuery>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException<TaskDto>(new NotFoundException("Task", 999)));

            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetTask(999));
        }
    }
}
