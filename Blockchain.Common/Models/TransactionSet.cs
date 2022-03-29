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
        private readonly List<Transaction> _transactions;
        
        public TransactionSet(List<Transaction> transactions)
        {
            _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
        }
        
        public string GetDataString()
        {
            return JsonConvert.SerializeObject(_transactions);
        }
    }
}