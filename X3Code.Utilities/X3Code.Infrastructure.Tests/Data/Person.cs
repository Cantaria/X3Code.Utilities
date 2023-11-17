using System;

namespace X3Code.Infrastructure.Tests.Data;

public class Person : Entity
{
    public Person()
    {
    }

    public Person(string name, string surname, DateTime birthday)
    {
        Name = name;
        Surname = surname;
        Birthday = birthday;
    }
        
    public override Guid EntityId { get; set; }
        
    public string? Name { get; set; }
        
    public string? Surname { get; set; }
        
    public DateTime Birthday { get; set; }
}