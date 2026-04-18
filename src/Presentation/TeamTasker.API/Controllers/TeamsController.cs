using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamTasker.Application.Common.Models;
using TeamTasker.Application.Teams.Commands.AddTeamMember;
using TeamTasker.Application.Teams.Commands.CreateTeam;
using TeamTasker.Application.Teams.Commands.DeleteTeam;
using TeamTasker.Application.Teams.Commands.RemoveTeamMember;
using TeamTasker.Application.Teams.Commands.UpdateTeam;
using TeamTasker.Application.Teams.Common;
using TeamTasker.Application.Teams.Queries.GetTeamById;
using TeamTasker.Application.Teams.Queries.GetTeams;

namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Controller for managing teams.
    /// </summary>
    [Authorize]
    public class TeamsController : ApiControllerBase
    {
        /// <summary>
        /// Get teams with pagination and filtering.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<TeamDto>>> GetTeams(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null,
            [FromQuery] string department = null)
        {
            var result = await Mediator.Send(new GetTeamsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Department = department
            });

            return Ok(result);
        }

        /// <summary>
        /// Get a team by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeamDetailDto>> GetTeam(int id)
        {
            var team = await Mediator.Send(new GetTeamByIdQuery { Id = id });
            return Ok(team);
        }

        /// <summary>
        /// Create a new team.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> Create(CreateTeamCommand command)
        {
            var id = await Mediator.Send(command);

            return CreatedAtAction(nameof(GetTeam), new { id }, id);
        }

        /// <summary>
        /// Update an existing team.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(int id, UpdateTeamCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(CreateProblemResponse(
                    StatusCodes.Status400BadRequest,
                    "Bad Request",
                    "Team id in route and body must match."));
            }

            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Delete an existing team.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteTeamCommand { Id = id });
            return NoContent();
        }

        /// <summary>
        /// Add a member to a team.
        /// </summary>
        [HttpPost("{id:int}/members")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TeamMemberDto>> AddMember(int id, AddTeamMemberRequest request)
        {
            var member = await Mediator.Send(new AddTeamMemberCommand
            {
                TeamId = id,
                UserId = request.UserId,
                Role = request.Role
            });

            return Ok(member);
        }

        /// <summary>
        /// Remove a member from a team.
        /// </summary>
        [HttpDelete("{id:int}/members/{userId:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveMember(int id, int userId)
        {
            await Mediator.Send(new RemoveTeamMemberCommand
            {
                TeamId = id,
                UserId = userId
            });

            return NoContent();
        }
    }

    /// <summary>
    /// Request model for adding a team member.
    /// </summary>
    public class AddTeamMemberRequest
    {
        public int UserId { get; set; }
        public string Role { get; set; } = "Member";
    }
}
