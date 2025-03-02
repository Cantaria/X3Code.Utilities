namespace X3Code.Repository.Test.Entities;

public class VehiclePart : Entity<int>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
}