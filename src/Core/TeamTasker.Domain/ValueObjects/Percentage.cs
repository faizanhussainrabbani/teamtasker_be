using System;
using System.Collections.Generic;
using TeamTasker.Domain.Exceptions;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing a percentage value
    /// </summary>
    public class Percentage : ValueObject
    {
        public int Value { get; }

        private Percentage(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a new Percentage
        /// </summary>
        public static Percentage Create(int value)
        {
            if (value < 0 || value > 100)
                throw new DomainException("Percentage value must be between 0 and 100");

            return new Percentage(value);
        }

        /// <summary>
        /// Factory method to create a new Percentage from a decimal value
        /// </summary>
        public static Percentage FromDecimal(decimal value)
        {
            if (value < 0 || value > 1)
                throw new DomainException("Decimal percentage value must be between 0 and 1");

            return Create((int)Math.Round(value * 100));
        }

        /// <summary>
        /// Converts the percentage to a decimal value (0-1)
        /// </summary>
        public decimal ToDecimal()
        {
            return Value / 100m;
        }

        public static implicit operator int(Percentage percentage) => percentage.Value;

        public static explicit operator Percentage(int value) => Create(value);

        public override string ToString() => $"{Value}%";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
