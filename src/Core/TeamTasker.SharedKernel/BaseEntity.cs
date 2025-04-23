using System.Collections.Generic;
using MediatR;

namespace TeamTasker.SharedKernel
{
    /// <summary>
    /// Base class for all entities in the domain model
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        
        private List<INotification> _domainEvents = new List<INotification>();
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(INotification domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
