using System.Threading;
using System.Threading.Tasks;

namespace TeamTasker.SharedKernel.Interfaces
{
    /// <summary>
    /// Interface for domain event dispatcher
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync(BaseEntity entity, CancellationToken cancellationToken = default);
    }
}
