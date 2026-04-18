using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamTasker.Application.Common.Models;
using TeamTasker.Application.Tasks.Commands.CreateTask;
using TeamTasker.Application.Tasks.Commands.DeleteTask;
using TeamTasker.Application.Tasks.Commands.UpdateTask;
using TeamTasker.Application.Tasks.Commands.UpdateTaskProgress;
using TeamTasker.Application.Tasks.Commands.UpdateTaskStatus;
using TeamTasker.Application.Tasks.Common;
using TeamTasker.Application.Tasks.Queries.GetTaskById;
using TeamTasker.Application.Tasks.Queries.GetTasks;

namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Controller for managing tasks.
    /// </summary>
    [Authorize]
    public class TasksController : ApiControllerBase
    {
        /// <summary>
        /// Get tasks with filtering and pagination.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<TaskDto>>> GetTasks([FromQuery] GetTasksQuery query)
        {
            return await Mediator.Send(query);
        }

        /// <summary>
        /// Get a task by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            return await Mediator.Send(new GetTaskByIdQuery { Id = id });
        }

        /// <summary>
        /// Create a new task.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskDto>> Create(CreateTaskCommand command)
        {
            var createdTask = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
        }

        /// <summary>
        /// Update an existing task.
        /// </summary>
        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> Update(int id, UpdateTaskCommand command)
        {
            if (command.Id != 0 && command.Id != id)
            {
                return BadRequest(CreateProblemResponse(
                    StatusCodes.Status400BadRequest,
                    "Bad Request",
                    "Task id in route and body must match."));
            }

            command.Id = id;
            var updatedTask = await Mediator.Send(command);
            return Ok(updatedTask);
        }

        /// <summary>
        /// Update task status.
        /// </summary>
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> UpdateStatus(int id, UpdateTaskStatusRequest request)
        {
            var updatedTask = await Mediator.Send(new UpdateTaskStatusCommand
            {
                Id = id,
                Status = request?.Status
            });

            return Ok(updatedTask);
        }

        /// <summary>
        /// Update task progress.
        /// </summary>
        [HttpPatch("{id:int}/progress")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskDto>> UpdateProgress(int id, UpdateTaskProgressRequest request)
        {
            var updatedTask = await Mediator.Send(new UpdateTaskProgressCommand
            {
                Id = id,
                Progress = request?.Progress ?? 0
            });

            return Ok(updatedTask);
        }

        /// <summary>
        /// Delete a task.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteTaskCommand { Id = id });
            return NoContent();
        }
    }

    /// <summary>
    /// Request model for task status updates.
    /// </summary>
    public class UpdateTaskStatusRequest
    {
        public string Status { get; set; }
    }

    /// <summary>
    /// Request model for task progress updates.
    /// </summary>
    public class UpdateTaskProgressRequest
    {
        public int Progress { get; set; }
    }
}
