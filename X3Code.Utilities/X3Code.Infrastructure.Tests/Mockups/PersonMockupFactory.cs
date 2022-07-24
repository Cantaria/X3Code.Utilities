using System;
using System.Collections.Generic;
using X3Code.Infrastructure.Tests.Data;

namespace X3Code.Infrastructure.Tests.Mockups
{
    public class PersonMockupFactory
    {
        public static List<Person> GetNPersons(int n)
        {
            var result = new List<Person>();

            for (var i = 0; i < n; i++)
            {
                var person = new Person
                {
                    Birthday = new DateTime(1990, i / 12, i / 20),
                    Name = $"Name-{i}",
                    Surname = $"Surname-{i}"
                };
                result.Add(person);
            }
            
            return result;
        }
    }
}