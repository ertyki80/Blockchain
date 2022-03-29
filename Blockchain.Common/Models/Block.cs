using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Blockchain.Common.Models
{
    public class Block
    {
        public Block PreviousBlock { get; set;}
        public Hash Hash { get;set; }
        public Hash PreviousHash { get; set;}
        public int Number { get;set; }
        public int Nonce { get; set;}
        public List<Transaction> Transaction { get; set; }

        public Block(Block block)
        {
            this.Hash = block.Hash;
            this.Nonce = block.Nonce;
            this.Number = block.Number;
            this.Transaction = new List<Transaction>(block.Transaction);
            this.PreviousBlock = block.PreviousBlock;
            this.PreviousHash = block.PreviousHash;
        }

        public Block() { }
        public Block(
            int nonce,
            List<Transaction> transactions,
            Hash hash,
            Block previousBlock = null)
        {
            
            if (transactions == null) throw new ArgumentNullException(nameof(transactions));
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
            Transaction = new List<Transaction>(transactions);
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