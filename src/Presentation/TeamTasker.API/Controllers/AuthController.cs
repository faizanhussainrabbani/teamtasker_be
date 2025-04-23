using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Auth.Models;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;

namespace TeamTasker.API.Controllers
{
    /// <summary>
    /// Controller for authentication
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthController(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IPasswordHasher passwordHasher,
            ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        /// <summary>
        /// Login with username/email and password
        /// </summary>
        /// <param name="request">Login request</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> Login(AuthRequest request)
        {
            // Find user by username or email
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                user = await _userRepository.GetByEmailAsync(request.Username);
            }

            // Check if user exists and password is correct
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(user);

            _logger.LogInformation("User {UserId} ({Username}) logged in successfully", user.Id, user.Username);

            // Return authentication response
            return Ok(new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Token = token
            });
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">Registration request</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            // Check if username already exists
            if (await _userRepository.GetByUsernameAsync(request.Username) != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Check if email already exists
            if (await _userRepository.GetByEmailAsync(request.Email) != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Create new user
            var user = new User(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Username,
                passwordHash,
                "User");

            // Save user
            await _userRepository.AddAsync(user);

            // Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(user);

            _logger.LogInformation("New user registered: {UserId} ({Username})", user.Id, user.Username);

            // Return authentication response
            var response = new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Token = token
            };

            return CreatedAtAction(nameof(GetCurrentUser), new { id = user.Id }, response);
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> GetCurrentUser()
        {
            try
            {
                // Get user ID from claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    userIdClaim = User.FindFirst("sub")?.Value;
                }

                _logger.LogInformation("User claims: {Claims}", string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Get user from repository
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                // Return user information
                return Ok(new AuthResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Token = null // Don't return token for this endpoint
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCurrentUser");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }
    }
}
