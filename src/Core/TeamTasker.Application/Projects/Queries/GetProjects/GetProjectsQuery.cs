using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Domain.Interfaces;

namespace TeamTasker.Application.Projects.Queries.GetProjects
{
    /// <summary>
    /// Query to get projects with pagination and filtering
    /// </summary>
    public class GetProjectsQuery : IRequest<PaginatedList<ProjectDto>>
    {
        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Optional search term to filter projects by name or description
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Optional status filter
        /// </summary>
        public string Status { get; set; }
    }

    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, PaginatedList<ProjectDto>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationDbContext _dbContext;

        public GetProjectsQueryHandler(
            IProjectRepository projectRepository,
            ICurrentUserService currentUserService,
            IApplicationDbContext dbContext)
        {
            _projectRepository = projectRepository;
            _currentUserService = currentUserService;
            _dbContext = dbContext;
        }

        public async Task<PaginatedList<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            // Start with all projects
            var query = _dbContext.Projects.AsQueryable();

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (System.Enum.TryParse<Domain.Entities.ProjectStatus>(request.Status, true, out var status))
                {
                    query = query.Where(p => p.Status == status);
                }
            }

            // Project to DTOs
            var projectDtosQuery = query.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status.ToString()
            });

            // Apply pagination
            return await PaginatedList<ProjectDto>.CreateAsync(
                projectDtosQuery,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }
}
