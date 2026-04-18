using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Projects.Queries.GetProjects;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TeamTasker.Application.Projects.Commands.UpdateProjectStatus
{
    /// <summary>
    /// Command to update the status of an existing project.
    /// </summary>
    public class UpdateProjectStatusCommand : IRequest<ProjectDto>
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }

    public class UpdateProjectStatusCommandHandler : IRequestHandler<UpdateProjectStatusCommand, ProjectDto>
    {
        private static readonly Dictionary<string, ProjectStatus> StatusMap =
            new Dictionary<string, ProjectStatus>(StringComparer.OrdinalIgnoreCase)
            {
                ["not-started"] = ProjectStatus.NotStarted,
                ["notstarted"] = ProjectStatus.NotStarted,
                ["in-progress"] = ProjectStatus.InProgress,
                ["inprogress"] = ProjectStatus.InProgress,
                ["completed"] = ProjectStatus.Completed,
                ["on-hold"] = ProjectStatus.OnHold,
                ["onhold"] = ProjectStatus.OnHold,
                ["cancelled"] = ProjectStatus.Cancelled
            };

        private readonly IProjectRepository _projectRepository;

        public UpdateProjectStatusCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectDto> Handle(UpdateProjectStatusCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);

            if (project == null)
            {
                throw new NotFoundException(nameof(Project), request.Id);
            }

            if (!TryParseStatus(request.Status, out var status))
            {
                throw new BadRequestException("Invalid project status value.");
            }

            project.UpdateStatus(status);
            await _projectRepository.UpdateAsync(project, cancellationToken);

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status.ToString()
            };
        }

        private static bool TryParseStatus(string rawStatus, out ProjectStatus status)
        {
            status = default;

            if (string.IsNullOrWhiteSpace(rawStatus))
            {
                return false;
            }

            var normalizedStatus = rawStatus.Trim();

            if (StatusMap.TryGetValue(normalizedStatus, out status))
            {
                return true;
            }

            return Enum.TryParse(normalizedStatus, true, out status);
        }
    }
}
