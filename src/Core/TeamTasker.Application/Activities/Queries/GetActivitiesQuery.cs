using MediatR;
using TeamTasker.Application.Common.Models;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Application.Activities.Queries
{
    /// <summary>
    /// Query to get activities with filtering and pagination
    /// </summary>
    public class GetActivitiesQuery : IRequest<PaginatedList<ActivityDto>>
    {
        public int? UserId { get; set; }
        public string? TargetType { get; set; }
        public int? TargetId { get; set; }
        public ActivityType? Action { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "desc";
    }

    /// <summary>
    /// Activity DTO for responses
    /// </summary>
    public class ActivityDto
    {
        public int Id { get; set; }
        public UserMinimalDto User { get; set; } = null!;
        public string Action { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public int? TargetId { get; set; }
        public string? TargetType { get; set; }
        public int? AssigneeId { get; set; }
        public UserMinimalDto? Assignee { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Timestamp { get; set; } = string.Empty;
    }

    /// <summary>
    /// Minimal user DTO for activity responses
    /// </summary>
    public class UserMinimalDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
    }
}
