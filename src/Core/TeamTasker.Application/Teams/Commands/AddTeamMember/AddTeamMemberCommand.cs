using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Teams.Common;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Application.Teams.Commands.AddTeamMember
{
    /// <summary>
    /// Command to add a user to a team.
    /// </summary>
    public class AddTeamMemberCommand : IRequest<TeamMemberDto>
    {
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = "Member";
    }

    public class AddTeamMemberCommandHandler : IRequestHandler<AddTeamMemberCommand, TeamMemberDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public AddTeamMemberCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TeamMemberDto> Handle(AddTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var teamExists = await _dbContext.Teams
                .AnyAsync(team => team.Id == request.TeamId, cancellationToken);

            if (!teamExists)
            {
                throw new NotFoundException(nameof(Team), request.TeamId);
            }

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(entity => entity.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.UserId);
            }

            var membershipExists = await _dbContext.TeamMembers
                .AnyAsync(
                    member => member.TeamId == request.TeamId && member.UserId == request.UserId,
                    cancellationToken);

            if (membershipExists)
            {
                throw new ConflictException("User is already a member of this team.");
            }

            var role = string.IsNullOrWhiteSpace(request.Role) ? "Member" : request.Role.Trim();
            var teamMember = new TeamMember(request.TeamId, request.UserId, role);

            await _dbContext.TeamMembers.AddAsync(teamMember, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new TeamMemberDto
            {
                UserId = user.Id,
                Name = user.FullName,
                Email = user.Email,
                Role = teamMember.Role,
                JoinedDate = teamMember.JoinedDate,
                Avatar = user.Avatar,
                Initials = user.Initials
            };
        }
    }
}
