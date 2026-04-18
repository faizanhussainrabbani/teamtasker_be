using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TeamTasker.API.Middleware;
using TeamTasker.Application.Common.Exceptions;
using Xunit;
using AppValidationException = TeamTasker.Application.Common.Exceptions.ValidationException;
using FluentValidationException = FluentValidation.ValidationException;

namespace TeamTasker.API.UnitTests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ShouldReturnBadRequest_ForApplicationValidationException()
        {
            var exception = new AppValidationException(new[]
            {
                new ValidationFailure("Progress", "Progress must be between 0 and 100")
            });

            var (statusCode, body) = await InvokeAndReadResponseAsync(exception);

            statusCode.Should().Be(StatusCodes.Status400BadRequest);
            body.GetProperty("status").GetInt32().Should().Be(StatusCodes.Status400BadRequest);
            body.GetProperty("title").GetString().Should().Be("Validation Error");
            body.GetProperty("detail").GetString().Should().NotBeNullOrWhiteSpace();
            body.TryGetProperty("message", out _).Should().BeFalse();

            var errors = body.GetProperty("errors");
            errors.ValueKind.Should().Be(JsonValueKind.Object);
            errors.GetProperty("Progress")[0].GetString().Should().Be("Progress must be between 0 and 100");
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnBadRequest_ForFluentValidationException()
        {
            var exception = new FluentValidationException(new[]
            {
                new ValidationFailure("Progress", "Progress must be between 0 and 100")
            });

            var (statusCode, body) = await InvokeAndReadResponseAsync(exception);

            statusCode.Should().Be(StatusCodes.Status400BadRequest);
            body.GetProperty("status").GetInt32().Should().Be(StatusCodes.Status400BadRequest);
            body.GetProperty("title").GetString().Should().Be("Validation Error");
            body.GetProperty("detail").GetString().Should().NotBeNullOrWhiteSpace();

            var errors = body.GetProperty("errors");
            errors.ValueKind.Should().Be(JsonValueKind.Object);
            errors.GetProperty("Progress")[0].GetString().Should().Be("Progress must be between 0 and 100");
        }

        [Fact]
        public async Task InvokeAsync_ShouldNormalizeUnnamedFluentValidationFailures_ToGeneralKey()
        {
            var exception = new FluentValidationException(new[]
            {
                new ValidationFailure(string.Empty, "General validation failure")
            });

            var (statusCode, body) = await InvokeAndReadResponseAsync(exception);

            statusCode.Should().Be(StatusCodes.Status400BadRequest);
            body.GetProperty("title").GetString().Should().Be("Validation Error");

            var errors = body.GetProperty("errors");
            errors.ValueKind.Should().Be(JsonValueKind.Object);
            errors.GetProperty("general")[0].GetString().Should().Be("General validation failure");
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnInternalServerError_ForUnexpectedException()
        {
            var exception = new Exception("Unexpected failure");

            var (statusCode, body) = await InvokeAndReadResponseAsync(exception);

            statusCode.Should().Be(StatusCodes.Status500InternalServerError);
            body.GetProperty("status").GetInt32().Should().Be(StatusCodes.Status500InternalServerError);
            body.GetProperty("title").GetString().Should().Be("Server Error");
            body.GetProperty("detail").GetString().Should().Be("Unexpected failure");
            body.GetProperty("errors").ValueKind.Should().Be(JsonValueKind.Null);
            body.TryGetProperty("message", out _).Should().BeFalse();
        }

        private static async Task<(int StatusCode, JsonElement Body)> InvokeAndReadResponseAsync(Exception exception)
        {
            var logger = Substitute.For<ILogger<ExceptionHandlingMiddleware>>();
            RequestDelegate next = _ => throw exception;
            var middleware = new ExceptionHandlingMiddleware(next, logger);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            var json = await reader.ReadToEndAsync();

            using var document = JsonDocument.Parse(json);
            return (context.Response.StatusCode, document.RootElement.Clone());
        }
    }
}