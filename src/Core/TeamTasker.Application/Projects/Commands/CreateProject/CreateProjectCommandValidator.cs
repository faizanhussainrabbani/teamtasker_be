using System;
using FluentValidation;

namespace TeamTasker.Application.Projects.Commands.CreateProject
{
    /// <summary>
    /// Validator for CreateProjectCommand
    /// </summary>
    public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(v => v.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(v => v.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(v => v.EndDate)
                .GreaterThanOrEqualTo(v => v.StartDate)
                .When(v => v.EndDate.HasValue)
                .WithMessage("End date must be greater than or equal to start date.");
        }
    }
}
