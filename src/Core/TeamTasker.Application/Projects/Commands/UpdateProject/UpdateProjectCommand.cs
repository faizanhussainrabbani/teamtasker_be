using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.Projects.Commands.UpdateProject
{
    /// <summary>
    /// Command to update an existing project
    /// </summary>
    public class UpdateProjectCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
    {
        private readonly IProjectRepository _projectRepository;

        public UpdateProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);

            if (project == null)
            {
                throw new NotFoundException(nameof(Project), request.Id);
            }

            project.UpdateDetails(
                request.Name,
                request.Description,
                request.StartDate,
                request.EndDate);

            await _projectRepository.UpdateAsync(project, cancellationToken);
        }
    }
}
