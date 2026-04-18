using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeamTasker.Application.Auth.Models;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Settings;

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
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordResetTokenService _passwordResetTokenService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;
        private readonly string _frontendBaseUrl;
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthController(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IRefreshTokenService refreshTokenService,
            IPasswordHasher passwordHasher,
            IPasswordResetTokenService passwordResetTokenService,
            IEmailService emailService,
            IOptions<AppSettings> appSettings,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenService = refreshTokenService;
            _passwordHasher = passwordHasher;
            _passwordResetTokenService = passwordResetTokenService;
            _emailService = emailService;
            _frontendBaseUrl = appSettings.Value.FrontendBaseUrl;
            _jwtSettings = jwtSettings.Value;
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
                return Unauthorized(CreateProblemResponse(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Invalid username or password"));
            }

            // Generate JWT token
            var token = _jwtTokenGenerator.GenerateToken(user);
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

            _logger.LogInformation("User {UserId} ({Username}) logged in successfully", user.Id, user.Username);

            // Return authentication response
            return Ok(CreateAuthResponse(user, token, refreshToken));
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">Registration request</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            if (await _userRepository.GetByUsernameAsync(request.Username) != null)
            {
                return Conflict(CreateProblemResponse(
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    "User with this username already exists"));
            }

            if (await _userRepository.GetByEmailAsync(request.Email) != null)
            {
                return Conflict(CreateProblemResponse(
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    "User with this email already exists"));
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
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

            _logger.LogInformation("New user registered: {UserId} ({Username})", user.Id, user.Username);

            // Return authentication response
            var response = CreateAuthResponse(user, token, refreshToken);

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
                    return Unauthorized(CreateProblemResponse(
                        StatusCodes.Status401Unauthorized,
                        "Unauthorized",
                        "Invalid token"));
                }

                // Get user from repository
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(CreateProblemResponse(
                        StatusCodes.Status401Unauthorized,
                        "Unauthorized",
                        "User not found"));
                }

                // Return user information
                return Ok(new AuthResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Token = null,
                    RefreshToken = null,
                    ExpiresAt = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCurrentUser");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    CreateProblemResponse(
                        StatusCodes.Status500InternalServerError,
                        "Server Error",
                        "An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Logout user
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Logout()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(CreateProblemResponse(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Invalid token"));
            }

            // Revoke all outstanding refresh tokens for this user to invalidate the session on logout.
            await _refreshTokenService.RevokeAllRefreshTokensAsync(userId);

            _logger.LogInformation("User {UserId} logged out and refresh tokens revoked", userId);

            return Ok(new
            {
                status = StatusCodes.Status200OK,
                title = "Success",
                detail = "Successfully logged out",
                errors = (object)null
            });
        }

        /// <summary>
        /// Refresh access token using a valid refresh token
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <returns>Authentication response with new access and refresh tokens</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Unauthorized(CreateProblemResponse(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Invalid refresh token"));
            }

            var userId = await _refreshTokenService.ValidateRefreshTokenAsync(request.RefreshToken);
            if (!userId.HasValue)
            {
                return Unauthorized(CreateProblemResponse(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Invalid or expired refresh token"));
            }

            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null)
            {
                await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);
                return Unauthorized(CreateProblemResponse(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "User not found"));
            }

            // Rotate token on each refresh request.
            await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);
            var newAccessToken = _jwtTokenGenerator.GenerateToken(user);
            var newRefreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

            _logger.LogInformation("Refreshed token for user {UserId}", user.Id);

            return Ok(CreateAuthResponse(user, newAccessToken, newRefreshToken));
        }

        /// <summary>
        /// Request password reset
        /// </summary>
        /// <param name="request">Forgot password request</param>
        /// <returns>Success message</returns>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    // Don't reveal that the user doesn't exist
                    return Ok(new { message = "Password reset email sent" });
                }

                // Generate password reset token
                var token = await _passwordResetTokenService.GeneratePasswordResetTokenAsync(user.Id, user.Email);

                // Send password reset email
                var resetUrl = $"{_frontendBaseUrl}/reset-password";
                await _emailService.SendPasswordResetEmailAsync(user.Email, token, resetUrl);

                _logger.LogInformation("Password reset requested for user {UserId} ({Email})", user.Id, user.Email);

                return Ok(new { message = "Password reset email sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword for email {Email}", request.Email);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    CreateProblemResponse(
                        StatusCodes.Status500InternalServerError,
                        "Server Error",
                        "An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Reset password with token
        /// </summary>
        /// <param name="request">Reset password request</param>
        /// <returns>Success message</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    return BadRequest(CreateProblemResponse(
                        StatusCodes.Status400BadRequest,
                        "Bad Request",
                        "Invalid token"));
                }

                // Validate token
                var isValidToken = await _passwordResetTokenService.ValidatePasswordResetTokenAsync(user.Id, user.Email, request.Token);
                if (!isValidToken)
                {
                    return BadRequest(CreateProblemResponse(
                        StatusCodes.Status400BadRequest,
                        "Bad Request",
                        "Invalid or expired token"));
                }

                // Hash new password
                var passwordHash = _passwordHasher.HashPassword(request.Password);

                // Update user password
                user.UpdatePassword(passwordHash);
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Password reset successful for user {UserId} ({Email})", user.Id, user.Email);

                return Ok(new { message = "Password reset successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPassword for email {Email}", request.Email);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    CreateProblemResponse(
                        StatusCodes.Status500InternalServerError,
                        "Server Error",
                        "An error occurred while processing your request"));
            }
        }

        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        /// <param name="request">Change password request</param>
        /// <returns>Success message</returns>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                // Get user ID from claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(CreateProblemResponse(
                        StatusCodes.Status401Unauthorized,
                        "Unauthorized",
                        "Invalid token"));
                }

                // Get user from repository
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(CreateProblemResponse(
                        StatusCodes.Status401Unauthorized,
                        "Unauthorized",
                        "User not found"));
                }

                // Verify current password
                if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                {
                    return BadRequest(CreateProblemResponse(
                        StatusCodes.Status400BadRequest,
                        "Bad Request",
                        "Current password is incorrect"));
                }

                // Hash new password
                var passwordHash = _passwordHasher.HashPassword(request.NewPassword);

                // Update user password
                user.UpdatePassword(passwordHash);
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Password changed for user {UserId} ({Username})", user.Id, user.Username);

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChangePassword");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    CreateProblemResponse(
                        StatusCodes.Status500InternalServerError,
                        "Server Error",
                        "An error occurred while processing your request"));
            }
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            userId = 0;

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                userIdClaim = User.FindFirst("sub")?.Value;
            }

            return !string.IsNullOrWhiteSpace(userIdClaim) && int.TryParse(userIdClaim, out userId);
        }

        private static object CreateProblemResponse(int status, string title, string detail, object errors = null)
        {
            return new
            {
                status,
                title,
                detail,
                errors
            };
        }

        private AuthResponse CreateAuthResponse(User user, string token, string refreshToken)
        {
            return new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = string.IsNullOrWhiteSpace(token)
                    ? null
                    : DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }
    }
}
