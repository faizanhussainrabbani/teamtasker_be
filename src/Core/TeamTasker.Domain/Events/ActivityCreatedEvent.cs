using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    /// <summary>
    /// Domain event raised when an activity is created
    /// </summary>
    public class ActivityCreatedEvent : INotification
    {
        public ActivityCreatedEvent(Activity activity)
        {
            Activity = activity;
        }

        public Activity Activity { get; }
    }
}
