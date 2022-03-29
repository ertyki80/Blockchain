using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blockchain.Common.Models;
using Newtonsoft.Json;

namespace Blockchain.Bll.Implementation
{
    public class BlockChainService: IDisposable
    {
        private readonly SHA256Managed _sha256 = new();
        private List<Block> _blockChain;
        public void Dispose() => _sha256.Dispose();
        private readonly int _startingZero;
        private List<Transaction> _currentTransactions;
        private Block _previousBlock;

        
        public BlockChainService(int startingZero = 0)
        {
            _startingZero = startingZero;
            _currentTransactions = new List<Transaction>();
            _blockChain = new List<Block>();
        }

        /// <summary>
        /// Validate transaction for amount money.
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="submitter"></param>
        /// <param name="amount"></param>
        /// <exception cref="Exception"></exception>
        private void ValidateTransaction(ref Person receiver,ref Person submitter,double amount)
        {
            if (submitter.AmountMoney < amount)
                throw new Exception("Submitter not have enough money");
            submitter.AmountMoney -= amount;
            receiver.AmountMoney += amount;
        }
        
        /// <summary>
        /// Create transaction from receiver to submitter
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="submitter"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Transaction CreateTransaction(ref Person receiver,ref Person submitter, double amount)
        {
            ValidateTransaction(ref receiver, ref submitter, amount);
            Transaction transaction = new Transaction(receiver, submitter, amount);
            //Add transaction for processing in generate block
            _currentTransactions.Add(transaction);
            return transaction;
        }

        /// <summary>
        /// Create new block of transaction
        /// </summary>
        /// <param name="previousBlock"></param>
        /// <param name="transactions"></param>
        /// <returns></returns>
        public Block GenerateBlock()
        {
           int currentNonce = 0;
           Hash hash = GenerateHash(ref _currentTransactions,ref currentNonce,ref _previousBlock);
            
            Block currentBlock = new Block(
                currentNonce,
                _currentTransactions,
                hash,
                _previousBlock
            );
            
            _blockChain.Add(currentBlock);
            _currentTransactions.Clear();
            _previousBlock = new Block(currentBlock);
            
            return currentBlock;
        }

        Hash GenerateHash(ref List<Transaction> transactionList,ref int nonce,ref Block previouslyBlock)
        {
            int currentNonce = 0;
            Console.WriteLine("Starting generate hash:");
            string transaction = JsonConvert.SerializeObject(transactionList);
            string zeros = new string('0', _startingZero);
            
            // Need some optimization:
            Hash newHash = null;
            while (true)
            {
                currentNonce++;
                string forProcessing = $"{currentNonce}_{transaction}";
                if(previouslyBlock != null)
                    forProcessing = $"{currentNonce}_{transaction}_{previouslyBlock.Hash.Value}";
                
                string hash = GetHashSha256(forProcessing);
                bool isValid = hash.StartsWith(zeros);
                if (isValid)
                {
                    newHash = new Hash(hash);
                    break;
                }
            }
            Console.WriteLine($"Generated hash: {newHash.Value}");
            nonce = currentNonce;
            return newHash;
        }
        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SHA256Managed sha256Managed = new SHA256Managed();
            byte[] hash = sha256Managed.ComputeHash(bytes);
            string result = string.Empty;
            foreach (var b in hash) result = result + String.Format("{0:x2}", b);
            return result;
        }
        
        public void SaveBlockChain(
            string name = null
        )
        {
            string jsonString = System.Text.Json.JsonSerializer.Serialize(_blockChain);
            DateTime now = DateTime.Now;
            
            string path =name ?? $"BlockChain({now.Day}_{now.Hour}_{now.Minute}).json";
            using FileStream file = File.OpenWrite(path);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            file.Write(bytes, 0, bytes.Length);
        }

        public void OpenBlockChain(string fileName)
        {
            using StreamReader r = new(fileName);
            List<Block> blocks = JsonConvert.DeserializeObject<List<Block>>(r.ReadToEnd());
            List<Block> validBlocks = ValidateBlockChain(blocks);
            if(blocks.Count == validBlocks.Count)
                Console.WriteLine("Blocks are valid");
        }

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
                _blockChain.AddRange(validBlocks);
            }
            return validBlocks;
        }
    }
}