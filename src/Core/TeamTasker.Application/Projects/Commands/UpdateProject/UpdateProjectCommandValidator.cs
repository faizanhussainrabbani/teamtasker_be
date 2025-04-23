using FluentValidation;

namespace TeamTasker.Application.Projects.Commands.UpdateProject
{
    /// <summary>
    /// Validator for UpdateProjectCommand
    /// </summary>
    public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("Id is required.");

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
