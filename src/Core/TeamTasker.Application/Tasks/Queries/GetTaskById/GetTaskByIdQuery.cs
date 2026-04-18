using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Tasks.Common;
using DomainTask = TeamTasker.Domain.Entities.Task;

namespace TeamTasker.Application.Tasks.Queries.GetTaskById
{
    /// <summary>
    /// Query to get a single task by id.
    /// </summary>
    public class GetTaskByIdQuery : IRequest<TaskDto>
    {
        public int Id { get; set; }
    }

    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetTaskByIdQueryHandler(
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserContext = await ResolveCurrentUserContextAsync(cancellationToken);

            var query = _dbContext.Tasks
                .AsNoTracking()
                .Include(t => t.AssignedToUser)
                .Include(t => t.Tags)
                .Where(t => t.Id == request.Id);

            query = ApplyVisibilityScope(query, currentUserContext);

            var task = await query.FirstOrDefaultAsync(cancellationToken);

            if (task == null)
            {
                throw new NotFoundException(nameof(DomainTask), request.Id);
            }

            return TaskContractMapper.ToDto(task);
        }

        private static IQueryable<DomainTask> ApplyVisibilityScope(
            IQueryable<DomainTask> query,
            CurrentTaskUserContext currentUserContext)
        {
            if (currentUserContext.IsAdmin)
            {
                return query;
            }

            if (currentUserContext.TeamIds.Count == 0)
            {
                return query.Where(task =>
                    task.AssignedToUserId == currentUserContext.UserId ||
                    (task.AssignedToUserId == null &&
                     task.Project != null &&
                     !task.Project.TeamId.HasValue));
            }

            return query.Where(task =>
                task.AssignedToUserId == currentUserContext.UserId ||
                (task.Project != null &&
                 task.Project.TeamId.HasValue &&
                 currentUserContext.TeamIds.Contains(task.Project.TeamId.Value)));
        }

        private async Task<CurrentTaskUserContext> ResolveCurrentUserContextAsync(CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                throw new ForbiddenAccessException("User context is required to access tasks.");
            }

            var userRole = await _dbContext.Users
                .AsNoTracking()
                .Where(user => user.Id == currentUserId.Value)
                .Select(user => user.Role)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(userRole))
            {
                throw new ForbiddenAccessException("Current user could not be resolved.");
            }

            var teamIds = await _dbContext.TeamMembers
                .AsNoTracking()
                .Where(teamMember => teamMember.UserId == currentUserId.Value)
                .Select(teamMember => teamMember.TeamId)
                .Distinct()
                .ToListAsync(cancellationToken);

            return new CurrentTaskUserContext
            {
                UserId = currentUserId.Value,
                IsAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase),
                TeamIds = teamIds
            };
        }

        private sealed class CurrentTaskUserContext
        {
            public int UserId { get; init; }
            public bool IsAdmin { get; init; }
            public List<int> TeamIds { get; init; } = new List<int>();
        }
    }
}
