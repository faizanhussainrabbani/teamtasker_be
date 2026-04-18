using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TeamTasker.API.Controllers;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Users.Models;
using TeamTasker.Domain.Interfaces;
using Xunit;
using User = TeamTasker.Domain.Entities.User;

namespace TeamTasker.API.UnitTests.Controllers
{
    public class UsersControllerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UsersController> _logger;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _dbContext = Substitute.For<IApplicationDbContext>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _logger = Substitute.For<ILogger<UsersController>>();

            _controller = new UsersController(
                _userRepository,
                _dbContext,
                _currentUserService,
                _logger);
        }

        [Fact]
        public async Task GetProfile_ShouldReturnUnauthorizedProblemShape_WhenCurrentUserMissing()
        {
            _currentUserService.UserId.Returns((int?)null);

            var result = await _controller.GetProfile();

            var unauthorized = result.Result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            AssertProblemShape(unauthorized.Value, 401, "Unauthorized", "Invalid token");
        }

        [Fact]
        public async Task GetUser_ShouldReturnNotFoundProblemShape_WhenUserDoesNotExist()
        {
            _userRepository.GetByIdAsync(Arg.Any<int>(), default).Returns((User)null);

            var result = await _controller.GetUser(999);

            var notFound = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            AssertProblemShape(notFound.Value, 404, "Resource Not Found", "User not found");
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnBadRequestProblemShape_WhenEmailIsAlreadyTaken()
        {
            var currentUser = new User("Current", "User", "current@example.com", "current-user") { Id = 1 };
            var existingUser = new User("Other", "User", "taken@example.com", "other-user") { Id = 2 };

            _currentUserService.UserId.Returns(1);
            _userRepository.GetByIdAsync(1, default).Returns(currentUser);
            _userRepository.GetByEmailAsync("taken@example.com", default).Returns(existingUser);

            var request = new UpdateProfileRequest
            {
                FirstName = "Current",
                LastName = "User",
                Email = "taken@example.com"
            };

            var result = await _controller.UpdateProfile(request);

            var badRequest = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            AssertProblemShape(badRequest.Value, 400, "Bad Request", "Email is already taken");
        }

        [Fact]
        public async Task GetUsers_ShouldReturnServerErrorProblemShape_WhenUnexpectedExceptionOccurs()
        {
            var result = await _controller.GetUsers(new UserFilterParams());

            var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(500);
            AssertProblemShape(objectResult.Value, 500, "Server Error", "An error occurred while processing your request");
        }

        private static void AssertProblemShape(object payload, int status, string title, string detail)
        {
            payload.Should().NotBeNull();

            var payloadType = payload.GetType();
            var statusValue = payloadType.GetProperty("status")?.GetValue(payload);
            var titleValue = payloadType.GetProperty("title")?.GetValue(payload);
            var detailValue = payloadType.GetProperty("detail")?.GetValue(payload);

            statusValue.Should().Be(status);
            titleValue.Should().Be(title);
            detailValue.Should().Be(detail);
        }
    }
}
