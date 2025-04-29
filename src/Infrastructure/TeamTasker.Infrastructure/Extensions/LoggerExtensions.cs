using System;
using Microsoft.Extensions.Logging;

namespace TeamTasker.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for ILogger
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Creates a logger instance for the specified type
        /// </summary>
        public static ILogger CreateLoggerInstance(this ILogger logger, Type type)
        {
            var loggerFactory = (ILoggerFactory)typeof(Logger<>)
                .Assembly
                .GetType("Microsoft.Extensions.Logging.LoggerFactoryExtensions")
                .GetMethod("CreateLoggerFactory", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .Invoke(null, new object[] { logger });

            return loggerFactory.CreateLogger(type);
        }
    }
}
