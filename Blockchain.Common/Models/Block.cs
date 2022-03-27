using System;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain.Common.Models
{
    public class Block
    {
        private readonly Lazy<string> _dataString;
        private Block PreviousBlock { get; }
        public Hash Hash { get; }
        public Hash PreviousHash { get; }
        public int Number { get; }
        public int Nonce { get; }
        public string DataString => _dataString.Value;
        
        public Block(
            int nonce,
            TransactionSet transactionSet,
            Hash hash,
            Block previousBlock = null)
        {
            if (transactionSet == null) throw new ArgumentNullException(nameof(transactionSet));
            if (hash == null) throw new ArgumentNullException(nameof(hash));

            if (previousBlock == null)
            {
                PreviousHash = new Hash(
                    string.Join(
                        string.Empty,
                        Enumerable.Range(0, 64).Select(x => "0")
                    )
                );
            }
            else
            {
                Number = previousBlock.Number + 1;
                PreviousHash = previousBlock.Hash;
            }
            Number = (previousBlock == null) ? 0 : (previousBlock.Number + 1);
            Nonce = nonce;
            Hash = hash;
            PreviousBlock = previousBlock;
            _dataString = new Lazy<string>(transactionSet.GetDataString);
        }

        public List<Block> GetChainAsList()
        {
            List<Block> blocks = new List<Block>();
            Block currentBlock = this;
            
            while (currentBlock != null)
            {
                blocks.Add(currentBlock);
                currentBlock = currentBlock.PreviousBlock;
            }
            return blocks;
        }

    }

}