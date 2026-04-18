using FluentValidation;

namespace TeamTasker.Application.Teams.Commands.UpdateTeam
{
    /// <summary>
    /// Validator for UpdateTeamCommand.
    /// </summary>
    public class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
    {
        public UpdateTeamCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0)
                .WithMessage("Team id is required.");

            RuleFor(command => command.Name)
                .NotEmpty()
                .WithMessage("Team name is required.")
                .MaximumLength(100)
                .WithMessage("Team name must not exceed 100 characters.");

            RuleFor(command => command.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");

            RuleFor(command => command.Department)
                .MaximumLength(100)
                .WithMessage("Department must not exceed 100 characters.");

            RuleFor(command => command.LeadId)
                .GreaterThan(0)
                .When(command => command.LeadId.HasValue)
                .WithMessage("Lead id must be greater than 0.");
        }
    }
}
