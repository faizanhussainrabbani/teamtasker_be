using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;
using TeamTasker.Application.Tasks.Common;
using DomainTask = TeamTasker.Domain.Entities.Task;

namespace TeamTasker.Application.Tasks.Queries.GetTasks
{
    /// <summary>
    /// Query to get paginated tasks with filters.
    /// </summary>
    public class GetTasksQuery : IRequest<PaginatedList<TaskDto>>
    {
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? AssigneeId { get; set; }
        public string? Search { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? SortBy { get; set; }
        public string SortDirection { get; set; } = "desc";
    }

    public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PaginatedList<TaskDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetTasksQueryHandler(
            IApplicationDbContext dbContext,
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedList<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            var currentUserContext = await ResolveCurrentUserContextAsync(cancellationToken);

            var query = _dbContext.Tasks
                .AsNoTracking()
                .Include(task => task.AssignedToUser)
                .Include(task => task.Tags)
                .AsQueryable();

            query = ApplyVisibilityScope(query, currentUserContext);

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                TaskContractMapper.TryParseStatus(request.Status, out var statusFilter))
            {
                query = query.Where(task => task.Status == statusFilter);
            }

            if (!string.IsNullOrWhiteSpace(request.Priority) &&
                TaskContractMapper.TryParsePriority(request.Priority, out var priorityFilter))
            {
                query = query.Where(task => task.Priority == priorityFilter);
            }

            if (!string.IsNullOrWhiteSpace(request.AssigneeId) && int.TryParse(request.AssigneeId, out var assigneeIdFilter))
            {
                query = query.Where(task => task.AssignedToUserId == assigneeIdFilter);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.Trim().ToLowerInvariant();
                query = query.Where(task =>
                    task.Title.ToLower().Contains(searchTerm) ||
                    task.Description.ToLower().Contains(searchTerm));
            }

            if (request.Tags != null && request.Tags.Count > 0)
            {
                var normalizedTags = request.Tags
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .Select(tag => tag.Trim().ToLowerInvariant())
                    .Distinct()
                    .ToList();

                if (normalizedTags.Count > 0)
                {
                    query = query.Where(task => task.Tags.Any(tag => normalizedTags.Contains(tag.Tag.ToLower())));
                }
            }

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var page = request.Page <= 0 ? 1 : request.Page;
            var limit = request.Limit <= 0 ? 10 : Math.Min(request.Limit, 100);

            var paginatedTasks = await PaginatedList<DomainTask>.CreateAsync(
                query,
                page,
                limit,
                cancellationToken);

            var taskDtos = paginatedTasks.Items
                .Select(TaskContractMapper.ToDto)
                .ToList();

            return new PaginatedList<TaskDto>(
                taskDtos,
                paginatedTasks.TotalCount,
                paginatedTasks.PageNumber,
                paginatedTasks.PageSize);
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

        private static IQueryable<DomainTask> ApplySorting(
            IQueryable<DomainTask> query,
            string? sortBy,
            string? sortDirection)
        {
            var normalizedSortBy = sortBy?.Trim().ToLowerInvariant();
            var ascending = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);

            return normalizedSortBy switch
            {
                "title" => ascending
                    ? query.OrderBy(task => task.Title)
                    : query.OrderByDescending(task => task.Title),
                "duedate" => ascending
                    ? query.OrderBy(task => task.DueDate)
                    : query.OrderByDescending(task => task.DueDate),
                "priority" => ascending
                    ? query.OrderBy(task => task.Priority)
                    : query.OrderByDescending(task => task.Priority),
                "status" => ascending
                    ? query.OrderBy(task => task.Status)
                    : query.OrderByDescending(task => task.Status),
                "createdat" => ascending
                    ? query.OrderBy(task => task.CreatedDate)
                    : query.OrderByDescending(task => task.CreatedDate),
                _ => ascending
                    ? query.OrderBy(task => task.UpdatedDate)
                    : query.OrderByDescending(task => task.UpdatedDate)
            };
        }
    }
}
