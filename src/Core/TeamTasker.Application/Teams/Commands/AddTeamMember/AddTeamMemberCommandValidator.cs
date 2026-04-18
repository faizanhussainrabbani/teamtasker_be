using FluentValidation;

namespace TeamTasker.Application.Teams.Commands.AddTeamMember
{
    /// <summary>
    /// Validator for AddTeamMemberCommand.
    /// </summary>
    public class AddTeamMemberCommandValidator : AbstractValidator<AddTeamMemberCommand>
    {
        public AddTeamMemberCommandValidator()
        {
            RuleFor(command => command.TeamId)
                .GreaterThan(0)
                .WithMessage("Team id is required.");

            RuleFor(command => command.UserId)
                .GreaterThan(0)
                .WithMessage("User id is required.");

            RuleFor(command => command.Role)
                .NotEmpty()
                .WithMessage("Role is required.")
                .MaximumLength(50)
                .WithMessage("Role must not exceed 50 characters.");
        }
    }
}
