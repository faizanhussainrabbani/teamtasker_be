using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Application.Teams.Commands.CreateTeam
{
    /// <summary>
    /// Command to create a team.
    /// </summary>
    public class CreateTeamCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public int? LeadId { get; set; }
    }

    public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, int>
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateTeamCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            if (request.LeadId.HasValue)
            {
                var leadExists = await _dbContext.Users
                    .AnyAsync(user => user.Id == request.LeadId.Value, cancellationToken);

                if (!leadExists)
                {
                    throw new NotFoundException(nameof(User), request.LeadId.Value);
                }
            }

            var team = new Team(
                request.Name,
                request.Description,
                request.Department,
                request.LeadId);

            await _dbContext.Teams.AddAsync(team, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return team.Id;
        }
    }
}
