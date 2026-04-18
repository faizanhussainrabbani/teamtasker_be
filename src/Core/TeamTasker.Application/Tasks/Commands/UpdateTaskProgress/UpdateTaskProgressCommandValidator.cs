using FluentValidation;

namespace TeamTasker.Application.Tasks.Commands.UpdateTaskProgress
{
    /// <summary>
    /// Validator for UpdateTaskProgressCommand.
    /// </summary>
    public class UpdateTaskProgressCommandValidator : AbstractValidator<UpdateTaskProgressCommand>
    {
        public UpdateTaskProgressCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0).WithMessage("Task id is required.");

            RuleFor(command => command.Progress)
                .InclusiveBetween(0, 100)
                .WithMessage("Progress must be between 0 and 100.");
        }
    }
}
