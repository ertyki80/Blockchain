using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Blockchain.Bll.Implementation;
using Blockchain.Common.Models;

namespace Blockchain.Console
{
    class Program
    {
        public static void RunTestOne()
        {
            int startingZero = 2;
            BlockChainGeneratorConsoleService generatorConsoleService = new(startingZero);
            PersonService personService = new PersonService();
            Block blockChain = generatorConsoleService.GenerateBlockChain(personService.GeneratePeople().ToList());
            
            System.Console.Clear();
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            foreach (Block block in blockChain
                .GetChainAsList()
                .OrderBy(x => x.Number))
            {
                stringBuilder.Append("{");
                stringBuilder.Append($"\"number\": \"{block.Number}\", ");
                stringBuilder.Append($"\"nonce\": \"{block.Nonce}\", ");
                stringBuilder.Append($"\"hash\": \"{block.Hash.Value}\", ");
                stringBuilder.Append($"\"previousHash\": \"{block.PreviousHash.Value}\", ");
                stringBuilder.Append($"\"Transactions\": \"{block.Transaction}\"");
                stringBuilder.Append("},");
                stringBuilder.AppendLine();
            }
            stringBuilder.Append("]");

            DateTime now = DateTime.Now;
            string path = $"{now.Day}_{now.Hour}_{now.Minute}.json";
            using (FileStream file = File.OpenWrite(path))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                file.Write(bytes, 0, bytes.Length);
            }
        }

        static void RunSecondTest()
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
        }
        
        
        
        static void TestDateGenerate()
        {
            Person personOne = new Person("Alice",200);
            Person personSecond = new Person("Bob",200);
            BlockChainService blockChainService = new BlockChainService(2);
            blockChainService.CreateTransaction(ref personOne,ref  personSecond, 31.2);
            blockChainService.CreateTransaction(ref personOne,ref  personSecond, 10.2);
            Block block = blockChainService.GenerateBlock();
            blockChainService.SaveBlockChain("Test.json");
        }
        
        static void TestForWorkAfterSave()
        {
            Person personOne = new Person("Alice",200);
            Person personSecond = new Person("Bob",200);
            BlockChainService blockChainService = new BlockChainService(2);
            blockChainService.CreateTransaction(ref personOne,ref  personSecond, 31.2);
            blockChainService.CreateTransaction(ref personOne, ref personSecond, 11.1);
            Block block3 = blockChainService.GenerateBlock();
            blockChainService.CreateTransaction(ref personOne,ref  personSecond, 12.2);
            blockChainService.CreateTransaction(ref personOne, ref personSecond, 9.1);
            Block block4 = blockChainService.GenerateBlock();
            blockChainService.SaveBlockChain("Test.json");
        }
        
        static void TestOpenBlockChain()
        {
            BlockChainService blockChainService = new BlockChainService(2);
            blockChainService.OpenBlockChain("Test.json");
        }
        
        static void Main(string[] args)
        {
            //RunTestOne();
            //RunSecondTest();
            TestForWorkAfterSave();

            TestOpenBlockChain();
            //TestDateGenerate();
            
        }
    }

}