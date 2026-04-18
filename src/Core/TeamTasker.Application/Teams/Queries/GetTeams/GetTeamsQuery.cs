using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Application.Teams.Common;

namespace TeamTasker.Application.Teams.Queries.GetTeams
{
    /// <summary>
    /// Query to retrieve teams with optional filtering.
    /// </summary>
    public class GetTeamsQuery : IRequest<PaginatedList<TeamDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public string Department { get; set; }
    }

    public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, PaginatedList<TeamDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetTeamsQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedList<TeamDto>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Teams
                .AsNoTracking()
                .Include(team => team.Lead)
                .Include(team => team.Members)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLowerInvariant();
                query = query.Where(team =>
                    team.Name.ToLower().Contains(searchTerm) ||
                    team.Description.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(request.Department))
            {
                var department = request.Department.Trim().ToLowerInvariant();
                query = query.Where(team => team.Department.ToLower() == department);
            }

            var dtoQuery = query
                .OrderBy(team => team.Name)
                .Select(team => new TeamDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    Department = team.Department,
                    LeadId = team.LeadId,
                    LeadName = team.Lead != null ? team.Lead.FullName : null,
                    MemberCount = team.Members.Count,
                    CreatedDate = team.CreatedDate,
                    UpdatedDate = team.UpdatedDate
                });

            return await PaginatedList<TeamDto>.CreateAsync(
                dtoQuery,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }
}
