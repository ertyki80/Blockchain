using System;
using System.Linq;
using Blockchain.Common.Enums;

namespace Blockchain.Common.Models
{
    public class Hash
    {
        public string Value { get; }
        public static Hash Empty => HashEmpty.EmptyHash;

        public Hash(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}