using MediatR;

namespace TeamTasker.Application.Activities.Queries
{
    /// <summary>
    /// Query to get a single activity by ID
    /// </summary>
    public class GetActivityByIdQuery : IRequest<ActivityDto?>
    {
        public int Id { get; set; }

        public GetActivityByIdQuery(int id)
        {
            Id = id;
        }
    }
}
