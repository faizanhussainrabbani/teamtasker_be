using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Domain.Interfaces;

namespace TeamTasker.Application.Projects.Queries.GetProjects
{
    /// <summary>
    /// Query to get all projects
    /// </summary>
    public class GetProjectsQuery : IRequest<List<ProjectDto>>
    {
    }

    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetProjectsQueryHandler(IProjectRepository projectRepository, ICurrentUserService currentUserService)
        {
            _projectRepository = projectRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectRepository.ListAllAsync(cancellationToken);
            var projectDtos = new List<ProjectDto>();

            foreach (var project in projects)
            {
                projectDtos.Add(new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Status = project.Status.ToString()
                });
            }

            return projectDtos;
        }
    }
}
