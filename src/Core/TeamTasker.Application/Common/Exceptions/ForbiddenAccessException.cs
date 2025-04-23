using System;

namespace TeamTasker.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a user attempts to access a resource they don't have permission for
    /// </summary>
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException() : base("You do not have permission to access this resource.")
        {
        }

        public ForbiddenAccessException(string message) : base(message)
        {
        }

        public ForbiddenAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
