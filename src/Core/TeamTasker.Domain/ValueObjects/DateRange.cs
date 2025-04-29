using System;
using System.Collections.Generic;
using TeamTasker.Domain.Exceptions;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing a date range
    /// </summary>
    public class DateRange : ValueObject
    {
        public DateTime Start { get; }
        public DateTime? End { get; }

        private DateRange(DateTime start, DateTime? end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Factory method to create a new DateRange
        /// </summary>
        public static DateRange Create(DateTime start, DateTime? end = null)
        {
            if (end.HasValue && end.Value < start)
                throw new DomainException("End date cannot be earlier than start date");

            return new DateRange(start, end);
        }

        /// <summary>
        /// Checks if a date is within the range
        /// </summary>
        public bool Includes(DateTime date)
        {
            return date >= Start && (!End.HasValue || date <= End.Value);
        }

        /// <summary>
        /// Checks if the range is in the past
        /// </summary>
        public bool IsInPast(DateTime referenceDate)
        {
            return End.HasValue && End.Value < referenceDate;
        }

        /// <summary>
        /// Checks if the range is in the future
        /// </summary>
        public bool IsInFuture(DateTime referenceDate)
        {
            return Start > referenceDate;
        }

        /// <summary>
        /// Calculates the duration of the range
        /// </summary>
        public TimeSpan? Duration()
        {
            return End.HasValue ? End.Value - Start : null;
        }

        public override string ToString()
        {
            return End.HasValue 
                ? $"{Start:yyyy-MM-dd} to {End.Value:yyyy-MM-dd}" 
                : $"{Start:yyyy-MM-dd} onwards";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
}
