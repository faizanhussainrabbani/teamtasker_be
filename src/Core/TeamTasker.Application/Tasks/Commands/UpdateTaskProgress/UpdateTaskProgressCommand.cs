using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Tasks.Common;
using DomainTask = TeamTasker.Domain.Entities.Task;

namespace TeamTasker.Application.Tasks.Commands.UpdateTaskProgress
{
    /// <summary>
    /// Command to update task progress.
    /// </summary>
    public class UpdateTaskProgressCommand : IRequest<TaskDto>
    {
        public int Id { get; set; }
        public int Progress { get; set; }
    }

    public class UpdateTaskProgressCommandHandler : IRequestHandler<UpdateTaskProgressCommand, TaskDto>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateTaskProgressCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskDto> Handle(UpdateTaskProgressCommand request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
            {
                throw new NotFoundException(nameof(DomainTask), request.Id);
            }

            task.UpdateProgress(request.Progress);
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
