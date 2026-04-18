using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TeamTasker.Application.Common.Interfaces;
using System.Linq;

namespace TeamTasker.Infrastructure.Services
{
    /// <summary>
    /// Service to get the current user from the HTTP context
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var claims = _httpContextAccessor.HttpContext?.User?.Claims;
                if (claims == null)
                {
                    return null;
                }

                var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                    ?? claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                return int.TryParse(userId, out var parsed) ? parsed : null;
            }
        }

        public string Username =>
            _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
            ?? _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
