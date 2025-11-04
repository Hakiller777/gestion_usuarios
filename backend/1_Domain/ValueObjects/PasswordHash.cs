using System;

namespace backend.Domain.ValueObjects
{
    public sealed record PasswordHash
    {
        public string Value { get; }

        private PasswordHash(string value)
        {
            Value = value;
        }

        public static PasswordHash FromHashed(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Password hash is required.", nameof(hash));
            return new PasswordHash(hash);
        }

        public override string ToString() => Value;
    }
}
