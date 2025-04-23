using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;

namespace TeamTasker.Application.Projects.Commands.CreateProject
{
    /// <summary>
    /// Command to create a new project
    /// </summary>
    public class CreateProjectCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
    {
        private readonly IProjectRepository _projectRepository;

        public CreateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = new Project(
                request.Name,
                request.Description,
                request.StartDate,
                request.EndDate);

            await _projectRepository.AddAsync(project, cancellationToken);

            return project.Id;
        }
    }
}
