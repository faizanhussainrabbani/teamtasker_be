using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.Teams.Commands.UpdateTeam
{
    /// <summary>
    /// Command to update team details.
    /// </summary>
    public class UpdateTeamCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public int? LeadId { get; set; }
    }

    public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateTeamCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await _dbContext.Teams
                .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);

            if (team == null)
            {
                throw new NotFoundException(nameof(Team), request.Id);
            }

            if (request.LeadId.HasValue)
            {
                var leadExists = await _dbContext.Users
                    .AnyAsync(user => user.Id == request.LeadId.Value, cancellationToken);

                if (!leadExists)
                {
                    throw new NotFoundException(nameof(User), request.LeadId.Value);
                }

                team.SetLead(request.LeadId.Value);
            }
            else
            {
                team.RemoveLead();
            }

            team.Update(request.Name, request.Description, request.Department);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
