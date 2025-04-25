using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;

namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Controller for team management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<TeamsController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public TeamsController(
            ITeamRepository teamRepository,
            ITeamMemberRepository teamMemberRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            ILogger<TeamsController> logger)
        {
            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// Get all teams
        /// </summary>
        /// <returns>List of teams</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<TeamDto>>> GetTeams()
        {
            try
            {
                _logger.LogInformation("Getting all teams");

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var teams = await _teamRepository.ListAllAsync();
                var teamDtos = teams.Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Department = t.Department,
                    LeadId = t.LeadId,
                    Lead = t.Lead != null ? new UserMinimalDto
                    {
                        Id = t.Lead.Id,
                        Name = $"{t.Lead.FirstName} {t.Lead.LastName}",
                        Avatar = t.Lead.Avatar,
                        Initials = t.Lead.Initials
                    } : null,
                    CreatedDate = t.CreatedDate,
                    UpdatedDate = t.UpdatedDate
                }).ToList();

                return Ok(teamDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teams");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get a team by ID
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <returns>Team details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TeamDetailDto>> GetTeam(int id)
        {
            try
            {
                _logger.LogInformation("Getting team {TeamId}", id);

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var team = await _teamRepository.GetTeamWithMembersAsync(id);
                if (team == null)
                {
                    return NotFound(new { message = "Team not found" });
                }

                var teamDto = new TeamDetailDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    Department = team.Department,
                    LeadId = team.LeadId,
                    Lead = team.Lead != null ? new UserMinimalDto
                    {
                        Id = team.Lead.Id,
                        Name = $"{team.Lead.FirstName} {team.Lead.LastName}",
                        Avatar = team.Lead.Avatar,
                        Initials = team.Lead.Initials
                    } : null,
                    Members = team.Members.Select(m => new TeamMemberDto
                    {
                        Id = m.Id,
                        UserId = m.UserId,
                        User = new UserMinimalDto
                        {
                            Id = m.User.Id,
                            Name = $"{m.User.FirstName} {m.User.LastName}",
                            Avatar = m.User.Avatar,
                            Initials = m.User.Initials
                        },
                        Role = m.Role,
                        JoinedDate = m.JoinedDate
                    }).ToList(),
                    CreatedDate = team.CreatedDate,
                    UpdatedDate = team.UpdatedDate
                };

                return Ok(teamDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team {TeamId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Create a new team
        /// </summary>
        /// <param name="request">Create team request</param>
        /// <returns>Created team</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TeamDto>> CreateTeam(CreateTeamRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new team");

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Validate lead if provided
                if (request.LeadId.HasValue)
                {
                    var lead = await _userRepository.GetByIdAsync(request.LeadId.Value);
                    if (lead == null)
                    {
                        return BadRequest(new { message = "Invalid lead ID" });
                    }
                }

                // Create team
                var team = new Team(request.Name, request.Description, request.Department, request.LeadId);
                await _teamRepository.AddAsync(team);

                // Add current user as a member if not already the lead
                if (request.LeadId != currentUserId)
                {
                    var teamMember = new TeamMember(team.Id, currentUserId.Value, "Admin");
                    await _teamMemberRepository.AddAsync(teamMember);
                }

                // Return created team
                var teamDto = new TeamDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    Department = team.Department,
                    LeadId = team.LeadId,
                    CreatedDate = team.CreatedDate,
                    UpdatedDate = team.UpdatedDate
                };

                return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, teamDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Update a team
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <param name="request">Update team request</param>
        /// <returns>No content</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateTeam(int id, UpdateTeamRequest request)
        {
            try
            {
                _logger.LogInformation("Updating team {TeamId}", id);

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Get team
                var team = await _teamRepository.GetByIdAsync(id);
                if (team == null)
                {
                    return NotFound(new { message = "Team not found" });
                }

                // Update team
                team.Update(request.Name, request.Description, request.Department);

                // Update lead if provided
                if (request.LeadId.HasValue)
                {
                    if (request.LeadId.Value == 0)
                    {
                        team.RemoveLead();
                    }
                    else
                    {
                        var lead = await _userRepository.GetByIdAsync(request.LeadId.Value);
                        if (lead == null)
                        {
                            return BadRequest(new { message = "Invalid lead ID" });
                        }

                        team.SetLead(request.LeadId.Value);
                    }
                }

                await _teamRepository.UpdateAsync(team);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team {TeamId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Delete a team
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteTeam(int id)
        {
            try
            {
                _logger.LogInformation("Deleting team {TeamId}", id);

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Get team
                var team = await _teamRepository.GetByIdAsync(id);
                if (team == null)
                {
                    return NotFound(new { message = "Team not found" });
                }

                await _teamRepository.DeleteAsync(team);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting team {TeamId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get team members
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <returns>List of team members</returns>
        [HttpGet("{teamId}/members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<TeamMemberDto>>> GetTeamMembers(int teamId)
        {
            try
            {
                _logger.LogInformation("Getting members for team {TeamId}", teamId);

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Check if team exists
                var team = await _teamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    return NotFound(new { message = "Team not found" });
                }

                // Get team members
                var teamMembers = await _teamMemberRepository.GetByTeamIdAsync(teamId);
                var teamMemberDtos = teamMembers.Select(m => new TeamMemberDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    User = new UserMinimalDto
                    {
                        Id = m.User.Id,
                        Name = $"{m.User.FirstName} {m.User.LastName}",
                        Avatar = m.User.Avatar,
                        Initials = m.User.Initials
                    },
                    Role = m.Role,
                    JoinedDate = m.JoinedDate
                }).ToList();

                return Ok(teamMemberDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting members for team {TeamId}", teamId);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Add a member to a team
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <param name="request">Add team member request</param>
        /// <returns>Created team member</returns>
        [HttpPost("{teamId}/members")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TeamMemberDto>> AddTeamMember(int teamId, AddTeamMemberRequest request)
        {
            try
            {
                _logger.LogInformation("Adding member to team {TeamId}", teamId);

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Check if team exists
                var team = await _teamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    return NotFound(new { message = "Team not found" });
                }

                // Check if user exists
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                // Check if user is already a member
                var existingMember = await _teamMemberRepository.GetByTeamAndUserIdAsync(teamId, request.UserId);
                if (existingMember != null)
                {
                    return BadRequest(new { message = "User is already a member of this team" });
                }

                // Add team member
                var teamMember = new TeamMember(teamId, request.UserId, request.Role);
                await _teamMemberRepository.AddAsync(teamMember);

                // Return created team member
                var teamMemberDto = new TeamMemberDto
                {
                    Id = teamMember.Id,
                    UserId = teamMember.UserId,
                    User = new UserMinimalDto
                    {
                        Id = user.Id,
                        Name = $"{user.FirstName} {user.LastName}",
                        Avatar = user.Avatar,
                        Initials = user.Initials
                    },
                    Role = teamMember.Role,
                    JoinedDate = teamMember.JoinedDate
                };

                return CreatedAtAction(nameof(GetTeamMember), new { teamId, memberId = teamMember.Id }, teamMemberDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding member to team {TeamId}", teamId);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get a team member
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <param name="memberId">Team member ID</param>
        /// <returns>Team member details</returns>
        [HttpGet("{teamId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TeamMemberDto>> GetTeamMember(int teamId, int memberId)
        {
            try
            {
                _logger.LogInformation("Getting team member {MemberId} for team {TeamId}", memberId, teamId);

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Get team member
                var teamMember = await _teamMemberRepository.GetByIdAsync(memberId);
                if (teamMember == null || teamMember.TeamId != teamId)
                {
                    return NotFound(new { message = "Team member not found" });
                }

                var teamMemberDto = new TeamMemberDto
                {
                    Id = teamMember.Id,
                    UserId = teamMember.UserId,
                    User = new UserMinimalDto
                    {
                        Id = teamMember.User.Id,
                        Name = $"{teamMember.User.FirstName} {teamMember.User.LastName}",
                        Avatar = teamMember.User.Avatar,
                        Initials = teamMember.User.Initials
                    },
                    Role = teamMember.Role,
                    JoinedDate = teamMember.JoinedDate
                };

                return Ok(teamMemberDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team member {MemberId} for team {TeamId}", memberId, teamId);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Update a team member's role
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <param name="memberId">Team member ID</param>
        /// <param name="request">Update team member request</param>
        /// <returns>No content</returns>
        [HttpPut("{teamId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> UpdateTeamMember(int teamId, int memberId, UpdateTeamMemberRequest request)
        {
            try
            {
                _logger.LogInformation("Updating team member {MemberId} for team {TeamId}", memberId, teamId);

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Get team member
                var teamMember = await _teamMemberRepository.GetByIdAsync(memberId);
                if (teamMember == null || teamMember.TeamId != teamId)
                {
                    return NotFound(new { message = "Team member not found" });
                }

                // Update role
                teamMember.UpdateRole(request.Role);
                await _teamMemberRepository.UpdateAsync(teamMember);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team member {MemberId} for team {TeamId}", memberId, teamId);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Remove a member from a team
        /// </summary>
        /// <param name="teamId">Team ID</param>
        /// <param name="memberId">Team member ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{teamId}/members/{memberId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RemoveTeamMember(int teamId, int memberId)
        {
            try
            {
                _logger.LogInformation("Removing team member {MemberId} from team {TeamId}", memberId, teamId);

                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Get team member
                var teamMember = await _teamMemberRepository.GetByIdAsync(memberId);
                if (teamMember == null || teamMember.TeamId != teamId)
                {
                    return NotFound(new { message = "Team member not found" });
                }

                await _teamMemberRepository.DeleteAsync(teamMember);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing team member {MemberId} from team {TeamId}", memberId, teamId);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }
    }
}
