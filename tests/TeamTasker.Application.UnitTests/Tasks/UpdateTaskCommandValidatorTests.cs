using FluentAssertions;
using TeamTasker.Application.Tasks.Commands.UpdateTask;
using Xunit;

namespace TeamTasker.Application.UnitTests.Tasks
{
    public class UpdateTaskCommandValidatorTests
    {
        private readonly UpdateTaskCommandValidator _validator = new UpdateTaskCommandValidator();

        [Fact]
        public void Validate_ShouldPass_WhenTagCountIsAtMostTwenty()
        {
            var command = new UpdateTaskCommand
            {
                Id = 1,
                Tags = Enumerable.Range(1, 20).Select(index => $"tag-{index}").ToList()
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldFail_WhenTagCountExceedsTwenty()
        {
            var command = new UpdateTaskCommand
            {
                Id = 1,
                Tags = Enumerable.Range(1, 21).Select(index => $"tag-{index}").ToList()
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(error =>
                error.ErrorMessage == "A task can have at most 20 tags.");
        }
    }
}
