using FluentValidation;

namespace TeamTasker.Application.Tasks.Commands.UpdateTask
{
    /// <summary>
    /// Validator for UpdateTaskCommand.
    /// </summary>
    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        private static readonly string[] AllowedStatuses =
        {
            "todo", "in-progress", "completed", "blocked", "on-hold", "cancelled"
        };

        private static readonly string[] AllowedPriorities =
        {
            "low", "medium", "high", "critical"
        };

        public UpdateTaskCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0).WithMessage("Task id is required.");

            RuleFor(command => command.Title)
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.")
                .When(command => command.Title != null);

            RuleFor(command => command.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
                .When(command => command.Description != null);

            RuleFor(command => command.Status)
                .Must(status => status == null || AllowedStatuses.Contains(status.Trim().ToLowerInvariant()))
                .WithMessage("Invalid status value.");

            RuleFor(command => command.Priority)
                .Must(priority => priority == null || AllowedPriorities.Contains(priority.Trim().ToLowerInvariant()))
                .WithMessage("Invalid priority value.");

            RuleFor(command => command.DueDate)
                .Must(dueDate => dueDate == null || System.DateTime.TryParse(dueDate, out _))
                .WithMessage("Invalid due date value.");

            RuleFor(command => command.AssigneeId)
                .Must(assigneeId => assigneeId == null || string.IsNullOrWhiteSpace(assigneeId) || int.TryParse(assigneeId, out _))
                .WithMessage("AssigneeId must be numeric.");

            RuleFor(command => command.Progress)
                .InclusiveBetween(0, 100)
                .When(command => command.Progress.HasValue)
                .WithMessage("Progress must be between 0 and 100.");

            RuleForEach(command => command.Tags)
                .MaximumLength(50).WithMessage("Each tag must not exceed 50 characters.");

            RuleFor(command => command.Tags)
                .Must(tags => tags == null || tags.Count <= 20)
                .WithMessage("A task can have at most 20 tags.");
        }
    }
}
