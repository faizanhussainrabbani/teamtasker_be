using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamTasker.Application.Common.Models;
using TeamTasker.Application.Projects.Commands.CreateProject;
using TeamTasker.Application.Projects.Commands.DeleteProject;
using TeamTasker.Application.Projects.Commands.UpdateProject;
using TeamTasker.Application.Projects.Queries.GetProjectById;
using TeamTasker.Application.Projects.Queries.GetProjects;

namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Controller for managing projects
    /// </summary>
    public class ProjectsController : ApiControllerBase
    {
        /// <summary>
        /// Get projects with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="searchTerm">Optional search term to filter projects by name or description</param>
        /// <param name="status">Optional status filter</param>
        /// <returns>Paginated list of projects</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<ProjectDto>>> GetProjects(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null,
            [FromQuery] string status = null)
        {
            return await Mediator.Send(new GetProjectsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Status = status
            });
        }

        /// <summary>
        /// Get a project by id
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns>Project details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectDetailDto>> GetProject(int id)
        {
            return await Mediator.Send(new GetProjectByIdQuery { Id = id });
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="command">Create project command</param>
        /// <returns>Created project id</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> Create(CreateProjectCommand command)
        {
            var id = await Mediator.Send(command);

            return CreatedAtAction(nameof(GetProject), new { id }, id);
        }

        /// <summary>
        /// Update an existing project
        /// </summary>
        /// <param name="id">Project id</param>
        /// <param name="command">Update project command</param>
        /// <returns>No content</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(int id, UpdateProjectCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Delete a project
        /// </summary>
        /// <param name="id">Project id</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteProjectCommand { Id = id });

            return NoContent();
        }
    }
}
