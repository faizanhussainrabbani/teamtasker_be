using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;

namespace TeamTasker.Application.Projects.Queries.GetProjectById
{
    /// <summary>
    /// Query to get a project by id
    /// </summary>
    public class GetProjectByIdQuery : IRequest<ProjectDetailDto>
    {
        public int Id { get; set; }
    }

    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDetailDto>
    {
        private readonly IProjectRepository _projectRepository;

        public GetProjectByIdQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectDetailDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetProjectWithTasksAsync(request.Id, cancellationToken);

            if (project == null)
            {
                throw new NotFoundException(nameof(Project), request.Id);
            }

            var projectDto = new ProjectDetailDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status.ToString()
            };

            foreach (var task in project.Tasks)
            {
                projectDto.Tasks.Add(new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status.ToString(),
                    Priority = task.Priority.ToString(),
                    AssignedToUserId = task.AssignedToUserId
                });
            }

            return projectDto;
        }
    }
}
