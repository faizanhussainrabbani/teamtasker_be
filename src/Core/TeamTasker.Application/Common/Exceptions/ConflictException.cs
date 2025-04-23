using System;

namespace TeamTasker.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a conflict occurs (e.g., duplicate resource)
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException() : base("A conflict occurred.")
        {
        }

        public ConflictException(string message) : base(message)
        {
        }

        public ConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConflictException(string name, object key) : base($"Entity \"{name}\" ({key}) already exists.")
        {
        }
    }
}
