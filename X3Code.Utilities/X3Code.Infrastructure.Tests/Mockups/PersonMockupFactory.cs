using System;
using System.Collections.Generic;
using X3Code.Infrastructure.Tests.Data;

namespace X3Code.Infrastructure.Tests.Mockups
{
    public static class PersonMockupFactory
    {
        public static List<Person> CreateNPersons(int n)
        {
            var result = new List<Person>();

            for (var i = 0; i < n; i++)
            {
                var month = i % 12 + 1;
                var day = i % 28 + 1;
                var person = new Person
                {
                    Birthday = new DateTime(1990, month, day),
                    Name = $"Name-{i}",
                    Surname = $"Surname-{i}"
                };
                result.Add(person);
            }
            
            return result;
        }
    }
}