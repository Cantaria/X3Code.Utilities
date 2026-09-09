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
                gadget_label nvarchar(50) NOT NULL,
                OwnerEntityId uniqueidentifier NULL
            );
            """);

        var repository = new GadgetRepository(context);
        var owner = new Person { EntityId = Guid.NewGuid(), Name = "Owner" };
        var gadgets = new List<Gadget>
        {
            new() { EntityId = Guid.NewGuid(), Label = "first", Owner = owner },
            new() { EntityId = Guid.NewGuid(), Label = "second", Owner = owner },
        };

        // Getrackt, damit EF die Shadow-FK setzt - ungetrackt ist sie schlicht nicht lesbar.
        context.AttachRange(gadgets);

        // Reflection haette hier eine Spalte "Label" und eine Spalte "Owner" erzeugt - die
        // Tabelle hat aber "gadget_label", und "OwnerEntityId" haette komplett gefehlt.
        var written = await repository.BulkWrite(gadgets);

        Assert.Equal(2, written);

        var stored = _verifier.GetFromDb("SELECT gadget_label, OwnerEntityId FROM Gadget ORDER BY gadget_label");
        Assert.Equal(2, stored.Rows.Count);
        Assert.Equal("first", stored.Rows[0][0]);
        Assert.Equal("second", stored.Rows[1][0]);

        // Der eigentliche Punkt: die Shadow-FK landet mit Wert in der Spalte, nicht als NULL.
        Assert.Equal(owner.EntityId, stored.Rows[0][1]);
        Assert.Equal(owner.EntityId, stored.Rows[1][1]);

        await context.Database.ExecuteSqlRawAsync("DROP TABLE Gadget;");
    }

    [Fact]
    public async Task An_untracked_shadow_property_is_refused_instead_of_written_as_null()
    {
        await using var context = new MappingContext(_database.ConnectionString);
        await context.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID('Gadget', 'U') IS NOT NULL DROP TABLE Gadget;
            CREATE TABLE Gadget (
                EntityId uniqueidentifier NOT NULL PRIMARY KEY,
                gadget_label nvarchar(50) NOT NULL,
                OwnerEntityId uniqueidentifier NULL
            );
            """);

        var repository = new GadgetRepository(context);
        var gadgets = new List<Gadget>
        {
            new() { EntityId = Guid.NewGuid(), Label = "untracked", Owner = new Person { EntityId = Guid.NewGuid(), Name = "Owner" } },
        };

        // Kein Attach: die Shadow-FK ist nicht lesbar. Frueher lief das durch und schrieb NULL,
        // BulkWrite meldete trotzdem Erfolg.
        var exception = await Assert.ThrowsAsync<NotSupportedException>(() => repository.BulkWrite(gadgets));

        Assert.Contains("OwnerEntityId", exception.Message);
        Assert.Contains("not tracked", exception.Message);

        var stored = _verifier.GetFromDb("SELECT COUNT(1) FROM Gadget");
        Assert.Equal(0, (int)stored.Rows[0][0]);

        await context.Database.ExecuteSqlRawAsync("DROP TABLE Gadget;");
    }

    [Fact]
    public async Task A_computed_column_is_left_to_the_database()
    {
        await using var context = new ComputedContext(_database.ConnectionString);
        await context.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID('Measurement', 'U') IS NOT NULL DROP TABLE Measurement;
            CREATE TABLE Measurement (
                EntityId uniqueidentifier NOT NULL PRIMARY KEY,
                Width int NOT NULL,
                Height int NOT NULL,
                Area AS ([Width] * [Height])
            );
            """);

        var repository = new MeasurementRepository(context);

        // Vorher: "SqlException: The column Area cannot be modified because it is either a
        // computed column or is the result of a UNION operator."
        var written = await repository.BulkWrite([
            new Measurement { EntityId = Guid.NewGuid(), Width = 3, Height = 4 },
        ]);

        Assert.Equal(1, written);

        var stored = _verifier.GetFromDb("SELECT Area FROM Measurement");
        Assert.Equal(12, stored.Rows[0][0]);

        await context.Database.ExecuteSqlRawAsync("DROP TABLE Measurement;");
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
            // Bewusst NICHT ignoriert: die Beziehung erzeugt die Shadow-FK "OwnerEntityId",
            // und genau die wurde vom alten Mapping stillschweigend als NULL geschrieben.
            gadget.HasOne(x => x.Owner).WithMany().HasForeignKey("OwnerEntityId");
            modelBuilder.Entity<Person>().ToTable("Person");
        }
    }

    public class Measurement : Entity
    {
        public override Guid EntityId { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        // Von der Datenbank berechnet - darf nicht mitgeschickt werden.
        public int Area { get; private set; }
    }

    private class ComputedContext : DbContext
    {
        private readonly string _connectionString;

        public ComputedContext(string connectionString) => _connectionString = connectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var measurement = modelBuilder.Entity<Measurement>();
            measurement.ToTable("Measurement");
            measurement.HasKey(x => x.EntityId);
            measurement.Property(x => x.Area).HasComputedColumnSql("[Width] * [Height]");
        }
    }

    private class MeasurementRepository : GenericRepository<Measurement, ComputedContext>
    {
        public MeasurementRepository(ComputedContext context) : base(context) { }
    }

    private class GadgetRepository : GenericRepository<Gadget, MappingContext>
    {
        public GadgetRepository(MappingContext context) : base(context) { }
    }

    #endregion
}
