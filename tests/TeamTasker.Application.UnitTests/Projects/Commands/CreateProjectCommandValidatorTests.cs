using System;
using FluentAssertions;
using TeamTasker.Application.Projects.Commands.CreateProject;
using Xunit;

namespace TeamTasker.Application.UnitTests.Projects.Commands
{
    public class CreateProjectCommandValidatorTests
    {
        private readonly CreateProjectCommandValidator _validator;

        public CreateProjectCommandValidatorTests()
        {
            _validator = new CreateProjectCommandValidator();
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenAllPropertiesAreValid()
        {
            // Arrange
            var command = new CreateProjectCommand
            {
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNameIsEmpty()
        {
            // Arrange
            var command = new CreateProjectCommand
            {
                Name = "",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.Name));
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNameExceedsMaxLength()
        {
            // Arrange
            var command = new CreateProjectCommand
            {
                Name = new string('A', 101), // 101 characters
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.Name));
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenDescriptionExceedsMaxLength()
        {
            // Arrange
            var command = new CreateProjectCommand
            {
                Name = "Test Project",
                Description = new string('A', 501), // 501 characters
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.Description));
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenStartDateIsEmpty()
        {
            // Arrange
            var command = new CreateProjectCommand
            {
                Name = "Test Project",
                Description = "Test Description",
                StartDate = default,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.StartDate));
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEndDateIsBeforeStartDate()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(10);
            var endDate = DateTime.UtcNow.AddDays(5); // 5 days before start date
            
            var command = new CreateProjectCommand
            {
                Name = "Test Project",
                Description = "Test Description",
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.EndDate));
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenEndDateIsNull()
        {
            // Arrange
            var command = new CreateProjectCommand
            {
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = null
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}
