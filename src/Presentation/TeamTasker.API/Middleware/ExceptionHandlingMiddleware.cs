using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TeamTasker.Application.Common.Exceptions;
using TeamTasker.Domain.Exceptions;

namespace TeamTasker.API.Middleware
{
    /// <summary>
    /// Middleware to handle exceptions globally
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred while executing the request");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = GetStatusCode(exception);
            var response = new
            {
                status = statusCode,
                title = GetTitle(exception),
                detail = exception.Message,
                errors = GetErrors(exception)
            };

            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ValidationException => (int)HttpStatusCode.BadRequest,
                NotFoundException => (int)HttpStatusCode.NotFound,
                ForbiddenAccessException => (int)HttpStatusCode.Forbidden,
                BadRequestException => (int)HttpStatusCode.BadRequest,
                ConflictException => (int)HttpStatusCode.Conflict,
                DomainException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };
        }

        private static string GetTitle(Exception exception)
        {
            return exception switch
            {
                ValidationException => "Validation Error",
                NotFoundException => "Resource Not Found",
                ForbiddenAccessException => "Forbidden",
                BadRequestException => "Bad Request",
                ConflictException => "Conflict",
                DomainException => "Domain Rule Violation",
                _ => "Server Error"
            };
        }

        private static object GetErrors(Exception exception)
        {
            if (exception is ValidationException validationException)
            {
                return validationException.Errors;
            }

            return null;
        }
    }
}
