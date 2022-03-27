using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Blockchain.Common.Models;

namespace Blockchain.Bll.Implementation
{
    public class BlockChainService: IDisposable
    {
        private readonly SHA256Managed _sha256 = new();
        public void Dispose() => _sha256.Dispose();
        private readonly int _startingZero;
        private List<Transaction> _transactions;


        public BlockChainService(int startingZero, List<Transaction> transactions)
        {
            _startingZero = startingZero;
            _transactions = transactions ?? new List<Transaction>();
        }

        public void ValidateTransaction(ref Person receiver,ref Person submitter,double amount)
        {
            if (submitter.AmountMoney < amount)
                throw new Exception("Submitter not have enough money");
            submitter.AmountMoney -= amount;
            receiver.AmountMoney += amount;
        }
        
        public Transaction CreateTransaction(ref Person receiver,ref Person submitter, double amount)
        {
            ValidateTransaction(ref receiver, ref submitter, amount);
            Transaction transaction = new Transaction(receiver, submitter, amount);
            _transactions.Add(transaction);
            return transaction;
        }

        public Block GenerateBlock(Block previousBlock)
        {
            TransactionSet transactionSet = new TransactionSet(_transactions);
            Hash previousHash = (previousBlock != null)
                ? previousBlock.Hash
                : Hash.Empty;
            
            int currentNonce = 0;
            
            Hash newHash = Enumerable.Range(0, int.MaxValue)
                .Select(_ => $"{currentNonce++}_{previousHash.Value}_{transactionSet.GetDataString()}")
                .Select(inputString => _sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString)))
                .Select(hashBytes => string.Join(string.Empty, hashBytes.Select(@byte => @byte.ToString("x2"))))
                .Select(hashString =>
                {
                    string zeros = new string('0', _startingZero);
                    bool isValid = hashString.StartsWith(zeros);
                    return isValid ? new Hash(hashString) : null;
                })
                .First(x => x != null);


            previousBlock = new Block(
                currentNonce,
                transactionSet,
                newHash,
                previousBlock
            );
            
            _transactions.Clear();
            return previousBlock;
        }
        
        
    }
}