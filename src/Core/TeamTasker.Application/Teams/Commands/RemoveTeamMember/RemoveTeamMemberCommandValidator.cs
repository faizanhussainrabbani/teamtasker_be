using FluentValidation;

namespace TeamTasker.Application.Teams.Commands.RemoveTeamMember
{
    /// <summary>
    /// Validator for RemoveTeamMemberCommand.
    /// </summary>
    public class RemoveTeamMemberCommandValidator : AbstractValidator<RemoveTeamMemberCommand>
    {
        public RemoveTeamMemberCommandValidator()
        {
            RuleFor(command => command.TeamId)
                .GreaterThan(0)
                .WithMessage("Team id is required.");

            RuleFor(command => command.UserId)
                .GreaterThan(0)
                .WithMessage("User id is required.");
        }
    }
}
