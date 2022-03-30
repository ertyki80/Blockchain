using System;
using Blockchain.Common.Models;

namespace Blockchain.Bll.Implementation
{
    public class TransactionService
    {   
        /// <summary>
        /// Create transaction from receiver to submitter
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="submitter"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Transaction CreateTransaction(Person receiver,Person submitter, double amount)
        {
            ValidateTransaction(receiver, submitter, amount);
            Transaction transaction = new Transaction(receiver, submitter, amount);
            //Add transaction for processing in generate block
            return transaction;
        }
        
        /// <summary>
        /// Validate transaction for amount money.
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="submitter"></param>
        /// <param name="amount"></param>
        /// <exception cref="Exception"></exception>
        private void ValidateTransaction(Person receiver,Person submitter,double amount)
        {
            if (submitter.AmountMoney < amount)
                throw new Exception("Submitter not have enough money");
            submitter.AmountMoney -= amount;
            receiver.AmountMoney += amount;
        }
        
    }
}