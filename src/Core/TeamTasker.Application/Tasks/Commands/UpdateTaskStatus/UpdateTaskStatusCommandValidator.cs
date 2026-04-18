using FluentValidation;

namespace TeamTasker.Application.Tasks.Commands.UpdateTaskStatus
{
    /// <summary>
    /// Validator for UpdateTaskStatusCommand.
    /// </summary>
    public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
    {
        private static readonly string[] AllowedStatuses =
        {
            "todo", "in-progress", "completed", "blocked", "on-hold", "cancelled"
        };

        public UpdateTaskStatusCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0).WithMessage("Task id is required.");

            RuleFor(command => command.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => AllowedStatuses.Contains(status.Trim().ToLowerInvariant()))
                .WithMessage("Invalid status value.");
        }
    }
}
