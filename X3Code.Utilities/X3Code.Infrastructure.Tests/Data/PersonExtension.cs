using System;
using System.Collections.Generic;
using System.Data;

namespace X3Code.Infrastructure.Tests.Data;

internal static class PersonExtension
{
    internal static Person ToPerson(this DataRow source)
    {
        return new Person
        {
            Name = (string)source[nameof(Person.Name)],
            Surname = (string)source[nameof(Person.Surname)],
            Birthday = (DateTime)source[nameof(Person.Birthday)],
            EntityId = (Guid)source[nameof(Person.EntityId)]
        };
    }

    internal static List<Person> ToPerson(this DataTable source)
    {
        var result = new List<Person>();

        foreach (DataRow dataRow in source.Rows)
        {
            result.Add(dataRow.ToPerson());
        }
            
        return result;
    }
}