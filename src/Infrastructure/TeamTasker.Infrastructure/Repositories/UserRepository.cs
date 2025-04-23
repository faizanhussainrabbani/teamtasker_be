using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TeamTasker.Domain.Entities;
using TeamTasker.Domain.Interfaces;
using TeamTasker.Infrastructure.Data;

namespace TeamTasker.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for User entity
    /// </summary>
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext dbContext, ILogger<UserRepository> logger) : base(dbContext, logger)
        {
            _logger = logger;
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<User>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Where(u => u.Role == role)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<User>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Where(u => u.Department == department)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return await GetAllAsync(cancellationToken);
            }

            var lowerSearchTerm = searchTerm.ToLower();

            return await _dbContext.Users
                .Where(u =>
                    u.FirstName.ToLower().Contains(lowerSearchTerm) ||
                    u.LastName.ToLower().Contains(lowerSearchTerm) ||
                    u.Email.ToLower().Contains(lowerSearchTerm) ||
                    u.Username.ToLower().Contains(lowerSearchTerm))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(cancellationToken);
        }
    }
}
