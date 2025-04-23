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
                var userId = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                return !string.IsNullOrEmpty(userId) ? int.Parse(userId) : null;
            }
        }

        public string Username => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
