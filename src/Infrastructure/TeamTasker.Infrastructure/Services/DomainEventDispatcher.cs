using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TeamTasker.SharedKernel;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Infrastructure.Services
{
    /// <summary>
    /// Implementation of domain event dispatcher using MediatR
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchEventsAsync(BaseEntity entity, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in entity.DomainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            entity.ClearDomainEvents();
        }
    }
}
