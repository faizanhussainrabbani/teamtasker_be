using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamTasker.Domain.Entities;

namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for the application database context
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<Project> Projects { get; }
        DbSet<Domain.Entities.Task> Tasks { get; }
        DbSet<User> Users { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
