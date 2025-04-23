using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Interfaces;

namespace TeamTasker.Application.Common.Behaviors
{
    /// <summary>
    /// Logging behavior for MediatR pipeline
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private readonly ICurrentUserService _currentUserService;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? 0;
            var userName = _currentUserService.Username ?? "Anonymous";

            _logger.LogInformation("Handling {RequestName} for {UserId} {UserName}", requestName, userId, userName);

            var response = await next();

            _logger.LogInformation("Handled {RequestName} for {UserId} {UserName}", requestName, userId, userName);

            return response;
        }
    }
}
