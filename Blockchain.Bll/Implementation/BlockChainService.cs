using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blockchain.Common.Models;
using Newtonsoft.Json;

namespace Blockchain.Bll.Implementation
{
    public class BlockChainService: IDisposable
    {
        /// <summary>
        /// Dispose sha instance
        /// </summary>
        public void Dispose() => _sha256.Dispose();
        
        /// <summary>
        /// Sha256 manager for encrypt data
        /// </summary>
        private readonly SHA256Managed _sha256;

        /// <summary>
        /// Configurable params for hash:
        /// ex: _startingZero:3 => 000b487b017aeb50ab08ca297cb009a05abac2c2e8a1220d3e23fe47183cc270
        /// </summary>
        private readonly int _startingZero;

        /// <summary>
        /// Constructor for Block Chain Service
        /// </summary>
        /// <param name="startingZero"></param>
        public BlockChainService(int startingZero = 0)
        {
            _sha256 = new();
            _startingZero = startingZero;
        }

        /// <summary>
        /// Create new block of transaction
        /// </summary>
        public Block GenerateBlock(List<Transaction> transactions, Block previouslyBlock = null)
        {
           int currentNonce = 0;
           
           Hash hash = GenerateHash(ref transactions,ref currentNonce, previouslyBlock?.Hash?.Value);
            
            Block currentBlock = new Block(
                currentNonce,
                transactions,
                hash,
                previouslyBlock
            );
            return currentBlock;
        }

        /// <summary>
        /// Generate hash based on:
        /// 1. "{currentNonce}_{transaction}"
        /// This is for a block chan without a previous block
        /// 2. "{currentNonce}_{transaction}_{PreviouslyHash}"
        /// This is for a block chan with a previous block (help verified order of blocks)
        /// </summary>
        /// <param name="transactionList"></param>
        /// <param name="nonce"></param>
        /// <param name="PreviouslyHash"></param>
        /// <returns></returns>
        Hash GenerateHash(ref List<Transaction> transactionList,ref int nonce, string PreviouslyHash = null)
        {
            int currentNonce = 0;
            Console.WriteLine("Starting generate hash:");
            string transaction = JsonConvert.SerializeObject(transactionList);
            string zeros = new string('0', _startingZero);
            
            Hash newHash = null;
            Parallel.For(0, int.MaxValue,
                (i,breakLoopState) =>
                {
                    currentNonce++;
                    string forProcessing = $"{currentNonce}_{transaction}";
                    if (PreviouslyHash != null)
                        forProcessing = $"{currentNonce}_{transaction}_{PreviouslyHash}";

                    string hash = GetHashSha256(forProcessing);
                    bool isValid = hash.StartsWith(zeros);
                    if (isValid)
                    {
                        newHash = new Hash(hash);
                        breakLoopState.Break();
                    }
                });

            Console.WriteLine($"Generated hash: {newHash.Value}");
            nonce = currentNonce;
            return newHash;
        }
        
        /// <summary>
        /// Just encrypt function
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SHA256Managed sha256Managed = new SHA256Managed();
            byte[] hash = sha256Managed.ComputeHash(bytes);
            string result = string.Empty;
            foreach (var b in hash) result = result + String.Format("{0:x2}", b);
            return result;
        }

        /// <summary>
        /// Save block chain to file
        /// </summary>
        /// <param name="lastBlockInChain"></param>
        /// <param name="fileName"></param>
        public void SaveBlockChain(Block lastBlockInChain,string fileName = null)
        {
            List<Block> blocks = new List<Block>();
            blocks.Add(lastBlockInChain);
            Block temp = lastBlockInChain;
            while (temp.PreviousBlock != null)
            {
                blocks.Add(temp.PreviousBlock);
                temp = temp.PreviousBlock;
            }
            blocks.Reverse();
            string jsonString = System.Text.Json.JsonSerializer.Serialize(blocks);
            DateTime now = DateTime.Now;
            string path =fileName ?? $"BlockChain ({now.Day}_{now.Hour}_{now.Minute}).json";
            using FileStream file = File.OpenWrite(path);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            file.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Open and validate block chain
        /// </summary>
        /// <param name="fileName"></param>
        public void OpenBlockChain(string fileName)
        {
            using StreamReader r = new(fileName);
            List<Block> blocks = JsonConvert.DeserializeObject<List<Block>>(r.ReadToEnd());
            List<Block> validBlocks = ValidateBlockChain(blocks);
            if(blocks != null && blocks.Count == validBlocks.Count)
                Console.WriteLine("BlockChain are valid");
        }

        /// <summary>
        /// Validate chain of block for transaction and previously hash
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns></returns>
        private List<Block> ValidateBlockChain(List<Block> blocks)
        {
            List<Block> validBlocks = new List<Block>();
            // Check for transaction and hash with nonce is valid:
            foreach (Block block in blocks)
            {
                int currentNonce = block.Nonce;
                List<Transaction> currentTransactions = block.Transaction;
                string stringForProcessing = $"{currentNonce}_{JsonConvert.SerializeObject(currentTransactions)}";
                if(block.PreviousBlock != null)
                    stringForProcessing = $"{currentNonce}_{JsonConvert.SerializeObject(currentTransactions)}_{block.PreviousBlock.Hash.Value}";
              
                string generateHash = GetHashSha256(stringForProcessing);
                string currentHash = block.Hash.Value;
                
                if (generateHash == currentHash)
                {
                    validBlocks.Add(block);
                }
            }
            return validBlocks;
        }
    }
}