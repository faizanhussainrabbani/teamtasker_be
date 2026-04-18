using FluentAssertions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using TeamTasker.API.Controllers;
using TeamTasker.Application.Auth.Models;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Settings;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.API.UnitTests.Controllers
{
    public class AuthControllerRefreshTokenTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordResetTokenService _passwordResetTokenService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;
        private readonly AuthController _controller;

        public AuthControllerRefreshTokenTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
            _refreshTokenService = Substitute.For<IRefreshTokenService>();
            _passwordHasher = Substitute.For<IPasswordHasher>();
            _passwordResetTokenService = Substitute.For<IPasswordResetTokenService>();
            _emailService = Substitute.For<IEmailService>();
            _logger = Substitute.For<ILogger<AuthController>>();

            var appSettings = Options.Create(new AppSettings { FrontendBaseUrl = "http://localhost:3000" });
            var jwtSettings = Options.Create(new JwtSettings
            {
                Secret = "test-secret-should-be-long-enough",
                Issuer = "teamtasker",
                Audience = "teamtasker-client",
                ExpiryMinutes = 60
            });

            _controller = new AuthController(
                _userRepository,
                _jwtTokenGenerator,
                _refreshTokenService,
                _passwordHasher,
                _passwordResetTokenService,
                _emailService,
                appSettings,
                jwtSettings,
                _logger);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnUnauthorized_WhenTokenIsInvalid()
        {
            _refreshTokenService.ValidateRefreshTokenAsync("invalid", default)
                .Returns(Task.FromResult<int?>(null));

            var result = await _controller.RefreshToken(new RefreshTokenRequest { RefreshToken = "invalid" });

            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnAuthResponse_WhenTokenIsValid()
        {
            const string existingRefreshToken = "existing-refresh";
            const string newRefreshToken = "new-refresh";
            const string newAccessToken = "new-access-token";

            var user = new User("Jane", "Doe", "jane@example.com", "jane", "hash") { Id = 42 };

            _refreshTokenService.ValidateRefreshTokenAsync(existingRefreshToken, default)
                .Returns(Task.FromResult<int?>(42));
            _userRepository.GetByIdAsync(42, default)
                .Returns(Task.FromResult(user));
            _jwtTokenGenerator.GenerateToken(user)
                .Returns(newAccessToken);
            _refreshTokenService.CreateRefreshTokenAsync(42, default)
                .Returns(Task.FromResult(newRefreshToken));

            var result = await _controller.RefreshToken(new RefreshTokenRequest { RefreshToken = existingRefreshToken });

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var payload = ok.Value.Should().BeAssignableTo<AuthResponse>().Subject;

            payload.Token.Should().Be(newAccessToken);
            payload.RefreshToken.Should().Be(newRefreshToken);
            payload.Id.Should().Be(42);
        }

        [Fact]
        public async Task Logout_ShouldRevokeAllRefreshTokens_WhenUserClaimIsValid()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                        new[] { new Claim("sub", "42") },
                        "TestAuth"))
                }
            };

            var result = await _controller.Logout();

            result.Should().BeOfType<OkObjectResult>();
            await _refreshTokenService.Received(1)
                .RevokeAllRefreshTokensAsync(42, default);
        }

        [Fact]
        public async Task Logout_ShouldReturnUnauthorized_WhenUserClaimIsMissing()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            var result = await _controller.Logout();

            result.Should().BeOfType<UnauthorizedObjectResult>();
            await _refreshTokenService.DidNotReceiveWithAnyArgs()
                .RevokeAllRefreshTokensAsync(default, default);
        }
    }
}
