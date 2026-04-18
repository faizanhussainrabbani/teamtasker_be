using FluentAssertions;
using TeamTasker.Application.Projects.Commands.UpdateProjectStatus;
using Xunit;

namespace TeamTasker.Application.UnitTests.Projects.Commands
{
    public class UpdateProjectStatusCommandValidatorTests
    {
        private readonly UpdateProjectStatusCommandValidator _validator = new UpdateProjectStatusCommandValidator();

        [Fact]
        public void Validate_ShouldPass_ForCanonicalStatus()
        {
            var result = _validator.Validate(new UpdateProjectStatusCommand
            {
                Id = 1,
                Status = "on-hold"
            });

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldPass_ForEnumStyleStatus()
        {
            var result = _validator.Validate(new UpdateProjectStatusCommand
            {
                Id = 1,
                Status = "InProgress"
            });

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldFail_ForUnknownStatus()
        {
            var result = _validator.Validate(new UpdateProjectStatusCommand
            {
                Id = 1,
                Status = "done"
            });

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(error =>
                error.ErrorMessage == "Invalid project status value.");
        }
    }
}
