using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using X3Code.Repository.Test.Entities;

namespace X3Code.Repository.Test.Context;

/// <summary>
/// The context the unit of work tests run on. It is deliberately not the in-memory one
/// (<see cref="X3AppContext"/>): the in-memory provider knows no transactions, so it could not tell a committed
/// write from a discarded one. SQLite is relational, keeps a real transaction and still lives entirely in RAM.
/// The connection is handed in and stays open for the whole test - an in-memory SQLite database exists exactly
/// as long as its connection does.
/// </summary>
public sealed class TransactionalContext : DbContext
{
    private readonly DbConnection _connection;

    public TransactionalContext(DbConnection connection)
    {
        _connection = connection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connection);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var person = modelBuilder.Entity<Person>();
        person.HasKey(x => x.EntityId);
        person.Property(x => x.Name).HasMaxLength(50);
        person.Property(x => x.Surname).HasMaxLength(50);

        var vehiclePart = modelBuilder.Entity<VehiclePart>();
        vehiclePart.HasKey(x => x.EntityId);
        vehiclePart.Property(x => x.Name).HasMaxLength(50);
        vehiclePart.Property(x => x.Description).HasMaxLength(50);

        base.OnModelCreating(modelBuilder);
    }
}
