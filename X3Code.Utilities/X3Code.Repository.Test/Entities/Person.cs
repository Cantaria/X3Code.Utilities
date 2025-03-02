namespace X3Code.Repository.Test.Entities;

public class Person : Entity<Guid>
{
    public required string Name { get; set; }
    public string? Surname { get; set; }
    public DateTime Birthday { get; set; }
}