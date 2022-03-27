using System.Collections.Generic;
using System.Linq;

namespace Blockchain.Common.Models
{
    public class Person
    {
        public string Name { get; }
        public double AmountMoney;
        public Person(string name,double amountMoney)
        {
            Name = name;
            AmountMoney = amountMoney;
        }
    }
}