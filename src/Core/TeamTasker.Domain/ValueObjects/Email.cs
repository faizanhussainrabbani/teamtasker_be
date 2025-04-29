using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TeamTasker.Domain.Exceptions;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing an email address
    /// </summary>
    public class Email : ValueObject
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a new Email
        /// </summary>
        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");

            email = email.Trim();

            if (!IsValidEmail(email))
                throw new DomainException($"Invalid email format: {email}");

            return new Email(email);
        }

        private static bool IsValidEmail(string email)
        {
            // Simple regex for email validation
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        public static implicit operator string(Email email) => email.Value;

        public static explicit operator Email(string email) => Create(email);

        public override string ToString() => Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }
    }
}
