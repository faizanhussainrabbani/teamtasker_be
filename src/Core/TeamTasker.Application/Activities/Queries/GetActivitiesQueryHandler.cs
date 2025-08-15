using MediatR;
using TeamTasker.Application.Common.Interfaces;
using TeamTasker.Application.Common.Models;

namespace TeamTasker.Application.Activities.Queries
{
    /// <summary>
    /// Handler for GetActivitiesQuery
    /// </summary>
    public class GetActivitiesQueryHandler : IRequestHandler<GetActivitiesQuery, PaginatedList<ActivityDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActivitiesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<ActivityDto>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
        {
            var activities = await _unitOfWork.ActivityRepository.GetActivitiesAsync(
                userId: request.UserId,
                targetType: request.TargetType,
                targetId: request.TargetId,
                action: request.Action,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                sortBy: request.SortBy,
                sortDirection: request.SortDirection,
                cancellationToken: cancellationToken);

            var activityDtos = activities.Items.Select(activity => new ActivityDto
            {
                Id = activity.Id,
                User = new UserMinimalDto
                {
                    Id = activity.User.Id,
                    Name = activity.User.FullName,
                    Avatar = activity.User.Avatar,
                    Initials = activity.User.Initials
                },
                Action = activity.Action.ToString().ToLower(),
                Target = activity.Target,
                TargetId = activity.TargetId,
                TargetType = activity.TargetType,
                AssigneeId = activity.AssigneeId,
                Assignee = activity.Assignee != null ? new UserMinimalDto
                {
                    Id = activity.Assignee.Id,
                    Name = activity.Assignee.FullName,
                    Avatar = activity.Assignee.Avatar,
                    Initials = activity.Assignee.Initials
                } : null,
                Comment = activity.Comment,
                CreatedDate = activity.CreatedDate,
                Timestamp = GetRelativeTime(activity.CreatedDate)
            }).ToList();

            return new PaginatedList<ActivityDto>(
                activityDtos,
                activities.TotalCount,
                activities.PageNumber,
                activities.PageSize);
        }

        private static string GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) != 1 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) != 1 ? "s" : "")} ago";

            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) != 1 ? "s" : "")} ago";
        }
    }
}
