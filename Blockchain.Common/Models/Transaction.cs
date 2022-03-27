using System;

namespace Blockchain.Common.Models
{
    public class Transaction
    {
        public Person Receiver { get; set; }
        public Person Submitter { get; set; }
        public double Amount { get; set; }

        public Transaction(
            Person receiver,
            Person submitter,
            double amount)
        {
            Receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
            Submitter = submitter ?? throw new ArgumentNullException(nameof(submitter));
            Amount = amount;
        }
    }
}