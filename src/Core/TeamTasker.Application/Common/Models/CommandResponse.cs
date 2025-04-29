using System.Collections.Generic;

namespace TeamTasker.Application.Common.Models
{
    /// <summary>
    /// Standard response model for command handlers
    /// </summary>
    /// <typeparam name="T">Type of the data returned</typeparam>
    public class CommandResponse<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public T Data { get; }
        public List<string> Errors { get; }

        protected CommandResponse(bool success, string message, T data, List<string> errors)
        {
            Success = success;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        /// <summary>
        /// Creates a successful response with data
        /// </summary>
        public static CommandResponse<T> Ok(T data, string message = "")
        {
            return new CommandResponse<T>(true, message, data, null!);
        }

        /// <summary>
        /// Creates a failed response with error message
        /// </summary>
        public static CommandResponse<T> Fail(string message, List<string> errors = null!)
        {
            return new CommandResponse<T>(false, message, default!, errors);
        }

        /// <summary>
        /// Creates a failed response with a single error
        /// </summary>
        public static CommandResponse<T> Fail(string message, string error)
        {
            return new CommandResponse<T>(false, message, default!, new List<string> { error });
        }
    }

    /// <summary>
    /// Standard response model for command handlers that don't return data
    /// </summary>
    public class CommandResponse : CommandResponse<object>
    {
        protected CommandResponse(bool success, string message, object data, List<string> errors)
            : base(success, message, data, errors)
        {
        }

        /// <summary>
        /// Creates a successful response
        /// </summary>
        public static CommandResponse Ok(string message = "Operation completed successfully")
        {
            return new CommandResponse(true, message, null!, null!);
        }

        /// <summary>
        /// Creates a failed response with error message
        /// </summary>
        public static new CommandResponse Fail(string message, List<string> errors = null!)
        {
            return new CommandResponse(false, message, null!, errors);
        }

        /// <summary>
        /// Creates a failed response with a single error
        /// </summary>
        public static new CommandResponse Fail(string message, string error)
        {
            return new CommandResponse(false, message, null!, new List<string> { error });
        }
    }
}
