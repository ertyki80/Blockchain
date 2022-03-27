using System.Collections.Generic;
using Blockchain.Common.Models;

namespace Blockchain.Bll.Implementation
{
    public class PersonService
    {
        public IEnumerable<Person> GeneratePeople()
        {
            List<Person> persons = new List<Person>()
            {
                new("Alisa",100),
                new("Bob",100),
                new("Ivan",100),
            };
            return persons;
        }
    }
}