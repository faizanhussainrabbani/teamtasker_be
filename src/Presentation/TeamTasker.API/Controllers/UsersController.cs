using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Application.Users.Models;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Domain.ValueObjects;
using TeamTasker.Infrastructure.Data;

namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Controller for user management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public UsersController(
            IUserRepository userRepository,
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService,
            ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of users with optional filtering
        /// </summary>
        /// <param name="filterParams">Filter parameters</param>
        /// <returns>List of users</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedList<UserDto>>> GetUsers([FromQuery] UserFilterParams filterParams)
        {
            try
            {
                // Start with all users
                var query = _dbContext.Users.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(filterParams.Role))
                {
                    query = query.Where(u => u.Role == filterParams.Role);
                }

                if (!string.IsNullOrEmpty(filterParams.Department))
                {
                    query = query.Where(u => u.Department == filterParams.Department);
                }

                if (!string.IsNullOrEmpty(filterParams.Search))
                {
                    var search = filterParams.Search.ToLower();
                    query = query.Where(u =>
                        u.FirstName.ToLower().Contains(search) ||
                        u.LastName.ToLower().Contains(search) ||
                        u.Email.ToLower().Contains(search) ||
                        u.Username.ToLower().Contains(search));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(filterParams.SortBy))
                {
                    query = filterParams.SortBy.ToLower() switch
                    {
                        "name" => filterParams.SortAscending
                            ? query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                            : query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName),
                        "email" => filterParams.SortAscending
                            ? query.OrderBy(u => u.Email)
                            : query.OrderByDescending(u => u.Email),
                        "username" => filterParams.SortAscending
                            ? query.OrderBy(u => u.Username)
                            : query.OrderByDescending(u => u.Username),
                        "role" => filterParams.SortAscending
                            ? query.OrderBy(u => u.Role)
                            : query.OrderByDescending(u => u.Role),
                        "department" => filterParams.SortAscending
                            ? query.OrderBy(u => u.Department)
                            : query.OrderByDescending(u => u.Department),
                        "created" => filterParams.SortAscending
                            ? query.OrderBy(u => u.CreatedDate)
                            : query.OrderByDescending(u => u.CreatedDate),
                        _ => filterParams.SortAscending
                            ? query.OrderBy(u => u.Id)
                            : query.OrderByDescending(u => u.Id)
                    };
                }
                else
                {
                    // Default sorting
                    query = query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
                }

                // Execute query with pagination
                var paginatedUsers = await PaginatedList<User>.CreateAsync(
                    query,
                    filterParams.PageNumber,
                    filterParams.PageSize);

                // Map to DTOs
                var userDtos = paginatedUsers.Items.Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    Email = u.Email,
                    Username = u.Username,
                    Role = u.Role,
                    Avatar = u.Avatar,
                    Initials = u.Initials,
                    Department = u.Department,
                    CreatedDate = u.CreatedDate,
                    UpdatedDate = u.UpdatedDate
                }).ToList();

                // Create new paginated list with DTOs
                var result = new PaginatedList<UserDto>(
                    userDtos,
                    paginatedUsers.TotalCount,
                    paginatedUsers.PageNumber,
                    paginatedUsers.PageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get a single user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDetailDto>> GetUser(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Load user skills
                var dbContext = _dbContext as DbContext;
                if (dbContext != null)
                {
                    await dbContext.Entry(user)
                        .Collection(u => u.Skills)
                        .LoadAsync();
                }

                // Map to DTO
                var userDto = new UserDetailDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Email = user.Email,
                    Username = user.Username,
                    Role = user.Role,
                    Avatar = user.Avatar,
                    Initials = user.Initials,
                    Department = user.Department,
                    Bio = user.Bio,
                    Location = user.Location,
                    Phone = user.Phone,
                    Address = AddressDto.FromDomain(user.Address),
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate,
                    Skills = user.Skills.Select(s => new UserSkillDto
                    {
                        Id = s.SkillId,
                        Name = s.Skill?.Name ?? "Unknown Skill",
                        Level = s.Level,
                        YearsOfExperience = s.YearsOfExperience
                    }).ToList()
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>Current user profile</returns>
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDetailDto>> GetProfile()
        {
            try
            {
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                return await GetUser(currentUserId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        /// <param name="request">Update profile request</param>
        /// <returns>Updated user profile</returns>
        [HttpPatch("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDetailDto>> UpdateProfile(UpdateProfileRequest request)
        {
            try
            {
                var currentUserId = _currentUserService.UserId;
                if (currentUserId == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var user = await _userRepository.GetByIdAsync(currentUserId.Value);
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                // Check if email is already taken by another user
                if (user.Email != request.Email)
                {
                    var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        return BadRequest(new { message = "Email is already taken" });
                    }
                }

                // Update user profile
                user.UpdateProfile(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Department,
                    request.Bio,
                    request.Location,
                    request.Phone);

                // Update address if provided
                if (request.Address != null)
                {
                    var address = new Address(
                        request.Address.Street ?? string.Empty,
                        request.Address.City ?? string.Empty,
                        request.Address.State ?? string.Empty,
                        request.Address.Country ?? string.Empty,
                        request.Address.ZipCode ?? string.Empty);

                    user.UpdateAddress(address);
                }

                await _userRepository.UpdateAsync(user);

                // Return updated profile
                return await GetProfile();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }
    }
}
