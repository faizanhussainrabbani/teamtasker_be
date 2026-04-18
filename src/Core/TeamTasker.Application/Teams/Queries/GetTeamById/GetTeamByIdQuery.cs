using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Teams.Common;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Application.Teams.Queries.GetTeamById
{
    /// <summary>
    /// Query to retrieve a team by id.
    /// </summary>
    public class GetTeamByIdQuery : IRequest<TeamDetailDto>
    {
        public int Id { get; set; }
    }

    public class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, TeamDetailDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetTeamByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TeamDetailDto> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
        {
            var team = await _dbContext.Teams
                .AsNoTracking()
                .Include(t => t.Lead)
                .Include(t => t.Members)
                    .ThenInclude(member => member.User)
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (team == null)
            {
                throw new NotFoundException(nameof(Team), request.Id);
            }

            return new TeamDetailDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                Department = team.Department,
                LeadId = team.LeadId,
                LeadName = team.Lead != null ? team.Lead.FullName : null,
                CreatedDate = team.CreatedDate,
                UpdatedDate = team.UpdatedDate,
                Members = team.Members
                    .OrderBy(member => member.User.FirstName)
                    .ThenBy(member => member.User.LastName)
                    .Select(member => new TeamMemberDto
                    {
                        UserId = member.UserId,
                        Name = member.User.FullName,
                        Email = member.User.Email,
                        Role = member.Role,
                        JoinedDate = member.JoinedDate,
                        Avatar = member.User.Avatar,
                        Initials = member.User.Initials
                    })
                    .ToList()
            };
        }
    }
}
