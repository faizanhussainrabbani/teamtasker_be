using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Tasks.Common;
using DomainTask = TeamTasker.Domain.Entities.Task;

namespace TeamTasker.Application.Tasks.Commands.UpdateTaskStatus
{
    /// <summary>
    /// Command to update task status.
    /// </summary>
    public class UpdateTaskStatusCommand : IRequest<TaskDto>
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }

    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateTaskStatusCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
            {
                throw new NotFoundException(nameof(DomainTask), request.Id);
            }

            if (!TaskContractMapper.TryParseStatus(request.Status, out var parsedStatus))
            {
                throw new BadRequestException("Invalid task status value.");
            }

            task.UpdateStatus(parsedStatus);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var updatedTask = await _dbContext.Tasks
                .AsNoTracking()
                .Include(t => t.AssignedToUser)
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (updatedTask == null)
            {
                throw new NotFoundException(nameof(DomainTask), request.Id);
            }

            return TaskContractMapper.ToDto(updatedTask);
        }
    }
}
