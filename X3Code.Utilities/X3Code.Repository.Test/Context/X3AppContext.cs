using Microsoft.EntityFrameworkCore;
using X3Code.Infrastructure.Extensions;
using X3Code.Repository.Test.Entities;

namespace X3Code.Repository.Test.Context;

public class X3AppContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("X3AppContext");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var person = modelBuilder.Entity<Person>();
        person.Property(x => x.Name).HasMaxLength(50);
        person.Property(x => x.Surname).HasMaxLength(50);
        person.HasKey(x => x.EntityId);
        
        var vehiclePart = modelBuilder.Entity<VehiclePart>();
        vehiclePart.HasKey(x => x.EntityId);
        vehiclePart.Property(x => x.Name).HasMaxLength(50);
        vehiclePart.Property(x => x.Description).HasMaxLength(50);
        vehiclePart.Property(x => x.Price).ConfigureDecimalColum();
        
        base.OnModelCreating(modelBuilder);
    }
}