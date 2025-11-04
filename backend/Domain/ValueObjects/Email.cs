using System;

namespace backend.Domain.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email is required.", nameof(value));

            var normalized = value.Trim().ToLowerInvariant();
            return new Email(normalized);
        }

        public override string ToString() => Value;
    }
}
