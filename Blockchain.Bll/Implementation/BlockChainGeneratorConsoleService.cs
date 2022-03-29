using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Blockchain.Common.Models;

namespace Blockchain.Bll.Implementation
{
    public class BlockChainGeneratorConsoleService : IDisposable
    {
        private readonly SHA256Managed _sha256 = new();
        public void Dispose() => _sha256.Dispose();
        private readonly int _startingZero;

        public BlockChainGeneratorConsoleService(int startingZero)
        {
            _startingZero = startingZero;
        }

        public List<Transaction> GenerateTransactionSet(Person receiver,List<Person> people, Random random)
        {
            var transactions =
                Enumerable
                    .Range(1, 2)
                    .Select(transaction => new Transaction(
                            receiver: receiver,
                            submitter: people.OrderBy(x => random.Next()).First(x => x != receiver),
                            amount: random.Next(10, 50)
                        )
                    )
                    .ToList();
            return transactions;
        }

        public Block GenerateBlockChain(List<Person> people)
        {
            Random rng = new();
            Person receiver = people.OrderBy(x => rng.Next()).First();
            return GenerateBlock(receiver,people);
        }

        public Block GenerateBlock(Person receiver, List<Person> people)
        {
            Random rng = new();
            Block previousBlock = null;

            List<Transaction> transactions = GenerateTransactionSet(receiver, people, rng);
            TransactionSet transactionSet = new(
                transactions
            );

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
                transactions,
                newHash,
                previousBlock
            );

            return previousBlock;
        }
    }
    
}