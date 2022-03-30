using System.Collections.Generic;
using Blockchain.Bll.Implementation;
using Blockchain.Common.Models;

namespace Blockchain.Console
{
    class Program
    {
        /*static void Test_GenerateBlockWithStartingZeroN_BlockChain()
        {
            for (int i = 0; i < 20; i++)
            {
                System.Console.WriteLine($"For starting zero : {i}");
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Person personOne = new Person("Alice", 200);
                Person personSecond = new Person("Bob", 200);
                BlockChainService blockChainService = new BlockChainService(i);
                blockChainService.CreateTransaction(ref personOne, ref personSecond, 10.2);
                blockChainService.CreateTransaction(ref personSecond, ref personOne, 22.2);
                blockChainService.CreateTransaction(ref personOne, ref personSecond, 12.1);
                blockChainService.CreateTransaction(ref personSecond, ref personOne, 3.2);
                Block block = blockChainService.GenerateBlock();
                watch.Stop();
                var elapsedMs = watch.Elapsed.TotalMilliseconds;
                System.Console.WriteLine($"Total Ms: {elapsedMs}");
            }
            //blockChainService.SaveBlockChain("Test.json");
        }*/
        
        
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
            // END BLOCK 1
            
            //BLOCK 2
            transactions.Add(transactionService.CreateTransaction( personSecond,  personOne, 10.0));
            transactions.Add(transactionService.CreateTransaction( personOne,  personSecond, 20.0));
            Block block2 = blockChainService.GenerateBlock(transactions,block1);
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