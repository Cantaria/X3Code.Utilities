using System;

namespace X3Code.Infrastructure.Tests.Data;

public class Person : Entity
{
    public override Guid EntityId { get; set; }
        
    public required string Name { get; set; }
        
    public string? Surname { get; set; }
        
    public DateTime Birthday { get; set; }
}