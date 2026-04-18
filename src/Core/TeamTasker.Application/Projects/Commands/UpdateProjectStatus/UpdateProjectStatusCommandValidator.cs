using FluentValidation;

namespace TeamTasker.Application.Projects.Commands.UpdateProjectStatus
{
    /// <summary>
    /// Validator for UpdateProjectStatusCommand.
    /// </summary>
    public class UpdateProjectStatusCommandValidator : AbstractValidator<UpdateProjectStatusCommand>
    {
        private static readonly string[] AllowedStatuses =
        {
            "not-started",
            "in-progress",
            "completed",
            "on-hold",
            "cancelled",
            "notstarted",
            "inprogress",
            "onhold",
            "NotStarted",
            "InProgress",
            "OnHold",
            "Cancelled",
            "Completed"
        };

        public UpdateProjectStatusCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0)
                .WithMessage("Project id is required.");

            RuleFor(command => command.Status)
                .NotEmpty()
                .WithMessage("Status is required.")
                .Must(status => AllowedStatuses.Contains(status.Trim(), System.StringComparer.OrdinalIgnoreCase))
                .WithMessage("Invalid project status value.");
        }
    }
}
