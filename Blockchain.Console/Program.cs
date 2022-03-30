using System.Collections.Generic;
using System.Diagnostics;
using Blockchain.Bll.Implementation;
using Blockchain.Common.Models;
using Newtonsoft.Json;

namespace Blockchain.Console
{
    class Program
    {
        static void Test_GenerateBlockWithStartingZeroN_BlockChain()
        {
            for (int i = 0; i < 20; i++)
            {
                System.Console.WriteLine($"For starting zero : {i}");
                //Service set up
                TransactionService transactionService = new TransactionService();
                BlockChainService blockChainService = new BlockChainService(2);
            
                //Persons:
                Person personOne = new Person("Alice",200);
                Person personSecond = new Person("Bob",200);
            
                // Send and received money:
                List<Transaction> transactions = new List<Transaction>();
            
                //BLOCK 1
                transactions.Add(transactionService.CreateTransaction( personOne,  personSecond, 31.2));
                transactions.Add(transactionService.CreateTransaction( personOne,  personSecond, 11.1));
                
                //Track time generation
                Stopwatch watch = Stopwatch.StartNew();
                Block block = blockChainService.GenerateBlock(transactions);
                watch.Stop();
                
                PrintBlock(block);
                var elapsedMs = watch.Elapsed.TotalMilliseconds;
                System.Console.WriteLine($"Total Ms: {elapsedMs}");
            }
            //blockChainService.SaveBlockChain("Test.json");
        }

        static void PrintBlock(Block block)
        {
            System.Console.WriteLine("Block:");
            System.Console.WriteLine($"Block hash: {block.Hash.Value}");
            System.Console.WriteLine($"Block nonce: {block.Nonce}");
            System.Console.WriteLine($"Block number: {block.Number}");
            System.Console.WriteLine($"Block Transaction: \n{JsonConvert.SerializeObject(block.Transaction)}");
            System.Console.WriteLine($"Previously Hash: {block.PreviousHash.Value}");
            System.Console.WriteLine($"Previously Block: [Too long... Check previously hash]");
        }
        
        static void Test_GenerateAndSave2Blocks_BlockChain()
        {
            //Service set up
            TransactionService transactionService = new TransactionService();
            BlockChainService blockChainService = new BlockChainService(2);
            
            //Persons:
            Person personOne = new Person("Alice",200);
            Person personSecond = new Person("Bob",200);
            
            // Send and received money:
            List<Transaction> transactions = new List<Transaction>();
            
            //BLOCK 1
            transactions.Add(transactionService.CreateTransaction( personOne,  personSecond, 31.2));
            transactions.Add(transactionService.CreateTransaction( personOne,  personSecond, 11.1));
            Block block1 = blockChainService.GenerateBlock(transactions);
            PrintBlock(block1);
            // END BLOCK 1
            
            //BLOCK 2
            transactions.Add(transactionService.CreateTransaction( personSecond,  personOne, 10.0));
            transactions.Add(transactionService.CreateTransaction( personOne,  personSecond, 20.0));
            Block block2 = blockChainService.GenerateBlock(transactions,block1);
            PrintBlock(block2);
            // END BLOCK 2
            
            blockChainService.SaveBlockChain(block2,"Test.json");
        }
        
        static void Test_ReadFromFile_BlockChain()
        {
            BlockChainService blockChainService = new BlockChainService(2);
            blockChainService.OpenBlockChain("Test.json");
        }
        
        static void Main(string[] args)
        {
            Test_GenerateAndSave2Blocks_BlockChain();
            //Test_GenerateBlockWithStartingZeroN_BlockChain();
        }
    }

}