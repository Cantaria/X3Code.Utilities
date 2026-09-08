using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using X3Code.Infrastructure.Tests.Data;
using X3Code.Infrastructure.Tests.Helper;
using X3Code.Infrastructure.Tests.Mockups;
using Xunit;

namespace X3Code.Infrastructure.Tests.Tests;

/// <summary>
/// Nails down the two BulkWrite defects: the copy used to run on a second connection of its
/// own, so it survived a rollback of the context's transaction, and it derived its columns by
/// reflecting over the entity - which picked up navigation properties and ignored renamed
/// columns, both of which make SqlBulkCopy fail on a column the table has not got.
/// </summary>
[Collection(SqlServerCollection.Name)]
public class BulkWriteMappingTest : IDisposable
{
    private readonly SqlServerFixture _database;
    private readonly DatabaseConnector _verifier;

    public BulkWriteMappingTest(SqlServerFixture database)
    {
        _database = database;
        _verifier = new DatabaseConnector(database.ConnectionString);
    }

    private int CountPersons()
    {
        var table = _verifier.GetFromDb("SELECT COUNT(1) FROM Person");

        return (int)table.Rows[0][0];
    }

    [Fact]
    public async Task BulkWriteJoinsAnOpenTransactionAndIsRolledBackWithIt()
    {
        await using var context = new UnitTestContext(
            new DbContextOptionsBuilder<UnitTestContext>().UseSqlServer(_database.ConnectionString).Options);

        var repository = new PersonRepository(context);

        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            var written = await repository.BulkWrite(PersonMockupFactory.CreateNPersons(50), "Person");
            Assert.Equal(50, written);

            await transaction.RollbackAsync();
        }

        // Vorher lief der Bulk-Insert auf einer eigenen Verbindung und ueberlebte das Rollback.
        Assert.Equal(0, CountPersons());
    }

    [Fact]
    public async Task BulkWriteCommitsWithTheTransaction()
    {
        await using var context = new UnitTestContext(
            new DbContextOptionsBuilder<UnitTestContext>().UseSqlServer(_database.ConnectionString).Options);

        var repository = new PersonRepository(context);

        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            await repository.BulkWrite(PersonMockupFactory.CreateNPersons(25), "Person");
            await transaction.CommitAsync();
        }

        Assert.Equal(25, CountPersons());
    }

    [Fact]
    public async Task BulkWriteUsesTheMappedColumnNamesAndSkipsNavigationProperties()
    {
        // Eigene Tabelle per SQL, damit weder Migration noch Model-Snapshot angefasst werden.
        await using var context = new MappingContext(_database.ConnectionString);
        await context.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID('Gadget', 'U') IS NOT NULL DROP TABLE Gadget;
            CREATE TABLE Gadget (
                EntityId uniqueidentifier NOT NULL PRIMARY KEY,
                gadget_label nvarchar(50) NOT NULL
            );
            """);

        var repository = new GadgetRepository(context);
        var gadgets = new List<Gadget>
        {
            new() { EntityId = Guid.NewGuid(), Label = "first", Owner = new Person { Name = "ignored" } },
            new() { EntityId = Guid.NewGuid(), Label = "second", Owner = new Person { Name = "ignored" } },
        };

        // Reflection haette hier eine Spalte "Label" und eine Spalte "Owner" erzeugt -
        // die Tabelle hat aber "gadget_label" und kein Owner.
        var written = await repository.BulkWrite(gadgets);

        Assert.Equal(2, written);

        var stored = _verifier.GetFromDb("SELECT gadget_label FROM Gadget ORDER BY gadget_label");
        Assert.Equal(2, stored.Rows.Count);
        Assert.Equal("first", stored.Rows[0][0]);
        Assert.Equal("second", stored.Rows[1][0]);

        await context.Database.ExecuteSqlRawAsync("DROP TABLE Gadget;");
    }

    public void Dispose()
    {
        _verifier.CleanDataBase();
        GC.SuppressFinalize(this);
    }

    #region Test model

    public class Gadget : Entity
    {
        public override Guid EntityId { get; set; }

        [Column("gadget_label")]
        public string Label { get; set; } = string.Empty;

        // Navigation property - darf nicht als Spalte auftauchen
        public Person? Owner { get; set; }
    }

    private class MappingContext : DbContext
    {
        private readonly string _connectionString;

        public MappingContext(string connectionString) => _connectionString = connectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var gadget = modelBuilder.Entity<Gadget>();
            gadget.ToTable("Gadget");
            gadget.HasKey(x => x.EntityId);
            gadget.Property(x => x.Label).HasMaxLength(50);
            gadget.Ignore(x => x.Owner);
        }
    }

    private class GadgetRepository : GenericRepository<Gadget, MappingContext>
    {
        public GadgetRepository(MappingContext context) : base(context) { }
    }

    #endregion
}
