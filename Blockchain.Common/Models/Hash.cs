using System;

namespace Blockchain.Common.Models
{
    public class Hash
    {
        public string Value { get; }

        public Hash(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}