using System.Threading;
using System.Threading.Tasks;

namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Service for issuing and validating refresh tokens.
    /// </summary>
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshTokenAsync(int userId, CancellationToken cancellationToken = default);
        Task<int?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task RevokeAllRefreshTokensAsync(int userId, CancellationToken cancellationToken = default);
    }
}
