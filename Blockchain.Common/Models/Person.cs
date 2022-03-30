namespace Blockchain.Common.Models
{
    public class Person
    {
        public string Name { get; set; }
        public double AmountMoney{ get; set; }
        public Person(string name,double amountMoney)
        {
            Name = name;
            AmountMoney = amountMoney;
        }
    }
}