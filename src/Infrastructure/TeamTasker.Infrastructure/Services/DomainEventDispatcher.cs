using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DomainEventDispatcher> _logger;

        public DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> logger = null)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task DispatchEventsAsync(BaseEntity entity, CancellationToken cancellationToken = default)
        {
            var events = entity.DomainEvents.ToArray();

            if (events.Any())
            {
                _logger?.LogInformation("Dispatching {Count} domain events for entity {EntityType} with id {EntityId}",
                    events.Length, entity.GetType().Name, entity.Id);

                foreach (var domainEvent in events)
                {
                    _logger?.LogInformation("Publishing domain event {EventType}", domainEvent.GetType().Name);
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                entity.ClearDomainEvents();

                _logger?.LogInformation("Finished dispatching domain events for entity {EntityType} with id {EntityId}",
                    entity.GetType().Name, entity.Id);
            }
        }
    }
}
