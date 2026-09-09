using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using X3Code.Repository.Test.Entities;

namespace X3Code.Repository.Test.Context;

/// <summary>
/// Retries on <see cref="TimeoutException"/>. SQLite's default strategy never retries, so the
/// earlier ExecuteAsync tests never went round the loop a second time - they would have been
/// green against a plain BeginAsync plus CompleteAsync.
/// </summary>
public sealed class TestRetryStrategy : ExecutionStrategy
{
    public TestRetryStrategy(ExecutionStrategyDependencies dependencies)
        : base(dependencies, maxRetryCount: 3, maxRetryDelay: TimeSpan.Zero)
    {
    }

    protected override bool ShouldRetryOn(Exception exception) => exception is TimeoutException;
}

/// <summary>
/// The same in-memory SQLite database as <see cref="TransactionalContext"/>, but configured with a
/// strategy that actually retries - the situation EnableRetryOnFailure creates on SQL Server.
/// </summary>
public sealed class RetryingContext : DbContext
{
    private readonly SqliteConnection _connection;

    public RetryingContext(SqliteConnection connection) => _connection = connection;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connection, sqlite => sqlite.ExecutionStrategy(d => new TestRetryStrategy(d)));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var person = modelBuilder.Entity<Person>();
        person.HasKey(x => x.EntityId);
        person.Property(x => x.Name).HasMaxLength(50);
        person.Property(x => x.Surname).HasMaxLength(50);

        base.OnModelCreating(modelBuilder);
    }
}
