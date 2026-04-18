using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.Teams.Commands.DeleteTeam
{
    /// <summary>
    /// Command to delete a team.
    /// </summary>
    public class DeleteTeamCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteTeamCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await _dbContext.Teams
                .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);

            if (team == null)
            {
                throw new NotFoundException(nameof(Team), request.Id);
            }

            _dbContext.Teams.Remove(team);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
