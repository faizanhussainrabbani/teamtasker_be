using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.Teams.Commands.RemoveTeamMember
{
    /// <summary>
    /// Command to remove a member from a team.
    /// </summary>
    public class RemoveTeamMemberCommand : IRequest
    {
        public int TeamId { get; set; }
        public int UserId { get; set; }
    }

    public class RemoveTeamMemberCommandHandler : IRequestHandler<RemoveTeamMemberCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public RemoveTeamMemberCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(RemoveTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var teamMember = await _dbContext.TeamMembers
                .FirstOrDefaultAsync(
                    member => member.TeamId == request.TeamId && member.UserId == request.UserId,
                    cancellationToken);

            if (teamMember == null)
            {
                throw new NotFoundException(nameof(TeamMember), $"{request.TeamId}:{request.UserId}");
            }

            _dbContext.TeamMembers.Remove(teamMember);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
