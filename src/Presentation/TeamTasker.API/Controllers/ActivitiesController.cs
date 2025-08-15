using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Activities.Queries;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Domain.Entities;

namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Controller for activity management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActivitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ActivitiesController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public ActivitiesController(
            IMediator mediator,
            ICurrentUserService currentUserService,
            ILogger<ActivitiesController> logger)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of activities with optional filtering
        /// </summary>
        /// <param name="filterParams">Filter parameters</param>
        /// <returns>List of activities</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedList<ActivityDto>>> GetActivities([FromQuery] ActivityFilterParams filterParams)
        {
            try
            {
                _logger.LogInformation("Getting activities with filters");

                // Get current user ID
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var query = new GetActivitiesQuery
                {
                    UserId = filterParams.UserId,
                    TargetType = filterParams.TargetType,
                    TargetId = filterParams.TargetId,
                    Action = !string.IsNullOrEmpty(filterParams.Action) && Enum.TryParse<ActivityType>(filterParams.Action, true, out var actionType) ? actionType : null,
                    PageNumber = filterParams.PageNumber,
                    PageSize = filterParams.PageSize,
                    SortBy = filterParams.SortBy,
                    SortDirection = filterParams.SortDirection
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activities");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get a single activity by ID
        /// </summary>
        /// <param name="id">Activity ID</param>
        /// <returns>Activity details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ActivityDto>> GetActivity(int id)
        {
            try
            {
                _logger.LogInformation("Getting activity {ActivityId}", id);

                var query = new GetActivityByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new { message = "Activity not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activity {ActivityId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }
    }

    /// <summary>
    /// Filter parameters for activities
    /// </summary>
    public class ActivityFilterParams
    {
        public int? UserId { get; set; }
        public string? TargetType { get; set; }
        public int? TargetId { get; set; }
        public string? Action { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "desc";
    }


}
