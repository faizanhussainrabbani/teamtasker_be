using MediatR;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Domain.Events
{
    public class UserCreatedEvent : INotification
    {
        public User User { get; }

        public UserCreatedEvent(User user)
        {
            User = user;
        }
    }

    public class UserUpdatedEvent : INotification
    {
        public User User { get; }

        public UserUpdatedEvent(User user)
        {
            User = user;
        }
    }

    public class UserAddressUpdatedEvent : INotification
    {
        public User User { get; }

        public UserAddressUpdatedEvent(User user)
        {
            User = user;
        }
    }

    public class UserDeactivatedEvent : INotification
    {
        public User User { get; }

        public UserDeactivatedEvent(User user)
        {
            User = user;
        }
    }

    public class UserActivatedEvent : INotification
    {
        public User User { get; }

        public UserActivatedEvent(User user)
        {
            User = user;
        }
    }
}
