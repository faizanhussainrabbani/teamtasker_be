using FluentValidation;

namespace TeamTasker.Application.Tasks.Commands.CreateTask
{
    /// <summary>
    /// Validator for CreateTaskCommand.
    /// </summary>
    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        private static readonly string[] AllowedStatuses =
        {
            "todo", "in-progress", "completed", "blocked", "on-hold", "cancelled"
        };

        private static readonly string[] AllowedPriorities =
        {
            "low", "medium", "high", "critical"
        };

        public CreateTaskCommandValidator()
        {
            RuleFor(command => command.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(command => command.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(command => command.Description != null);

            RuleFor(command => command.Status)
                .Must(status => string.IsNullOrWhiteSpace(status) || AllowedStatuses.Contains(status.Trim().ToLowerInvariant()))
                .WithMessage("Invalid status value.");

            RuleFor(command => command.Priority)
                .Must(priority => string.IsNullOrWhiteSpace(priority) || AllowedPriorities.Contains(priority.Trim().ToLowerInvariant()))
                .WithMessage("Invalid priority value.");

            RuleFor(command => command.DueDate)
                .Must(dueDate => string.IsNullOrWhiteSpace(dueDate) || System.DateTime.TryParse(dueDate, out _))
                .WithMessage("Invalid due date value.");

            RuleFor(command => command.AssigneeId)
                .Must(assigneeId => string.IsNullOrWhiteSpace(assigneeId) || int.TryParse(assigneeId, out _))
                .WithMessage("AssigneeId must be numeric.");

            RuleForEach(command => command.Tags)
                .MaximumLength(50).WithMessage("Each tag must not exceed 50 characters.");

            RuleFor(command => command.Tags)
                .Must(tags => tags == null || tags.Count <= 20)
                .WithMessage("A task can have at most 20 tags.");
        }
    }
}
