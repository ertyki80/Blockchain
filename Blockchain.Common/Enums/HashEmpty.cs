using System.Linq;
using Blockchain.Common.Models;

namespace Blockchain.Common.Enums
{
    public class HashEmpty
    {
        public static Hash EmptyHash = new Hash(string.Join(string.Empty, Enumerable.Range(0, 64).Select(i => "0")));
    }
}