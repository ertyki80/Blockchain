using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Blockchain.Common.Models
{
    [DataContract]
    public class TransactionSet
    {
        [DataMember]
        public List<Transaction> Transactions => _transactions;
        private readonly List<Transaction> _transactions;
        private readonly string _dataStringLazy;
        public TransactionSet(List<Transaction> transactions)
        {
            _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
            _dataStringLazy = JsonConvert.SerializeObject(Transactions);
        }
        
        public string GetDataString()
        {
            return _dataStringLazy; 
        }
    }
}