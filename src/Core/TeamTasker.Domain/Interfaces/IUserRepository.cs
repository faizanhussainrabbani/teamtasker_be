using System.Threading;
using System.Threading.Tasks;
using TeamTasker.Domain.Entities;
using TeamTasker.SharedKernel.Interfaces;

namespace TeamTasker.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for User entity
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    }
}
