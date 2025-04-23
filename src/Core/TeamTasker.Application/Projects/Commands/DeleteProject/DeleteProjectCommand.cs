using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;

namespace TeamTasker.Application.Projects.Commands.DeleteProject
{
    /// <summary>
    /// Command to delete a project
    /// </summary>
    public class DeleteProjectCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
    {
        private readonly IProjectRepository _projectRepository;

        public DeleteProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);

            if (project == null)
            {
                throw new NotFoundException(nameof(Project), request.Id);
            }

            await _projectRepository.DeleteAsync(project, cancellationToken);
        }
    }
}
