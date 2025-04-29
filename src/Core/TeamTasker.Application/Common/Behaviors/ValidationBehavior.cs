using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using TeamTasker.Application.Common.Models;

namespace TeamTasker.Application.Common.Behaviors
{
    /// <summary>
    /// Validation behavior for MediatR pipeline
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var failureMessages = failures.Select(f => f.ErrorMessage).ToList();
                var failureMessage = "Validation failed: " + string.Join(", ", failureMessages);

                // If the response type is CommandResponse<T> or CommandResponse, return a failed response
                if (typeof(TResponse).IsGenericType &&
                    typeof(TResponse).GetGenericTypeDefinition() == typeof(CommandResponse<>))
                {
                    // Create a failed response using reflection
                    var failMethod = typeof(TResponse).GetMethod("Fail", new[] { typeof(string), typeof(List<string>) });
                    return failMethod.Invoke(null, new object[] { failureMessage, failureMessages }) as TResponse;
                }
                else if (typeof(TResponse) == typeof(CommandResponse))
                {
                    return CommandResponse.Fail(failureMessage, failureMessages) as TResponse;
                }
                else
                {
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
}
