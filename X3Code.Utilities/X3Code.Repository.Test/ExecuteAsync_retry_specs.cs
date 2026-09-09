using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using X3Code.Repository.Test.Context;
using X3Code.Repository.Test.Entities;
using X3Code.Repository.Test.Repositories;
using Xunit;

namespace X3Code.Repository.Test;

/// <summary>
/// ExecuteAsync exists for contexts with a retrying execution strategy. These tests actually go
/// round the loop a second time — which the earlier ExecuteAsync tests never did, because SQLite's
/// default strategy does not retry.
/// </summary>
public class ExecuteAsync_retry_specs : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public ExecuteAsync_retry_specs()
    {
        _connection.Open();
        using var context = new RetryingContext(_connection);
        context.Database.EnsureCreated();
    }

    private static Person NewPerson(string name) => new()
    {
        EntityId = Guid.NewGuid(),
        Name = name,
        Birthday = new DateTime(2000, 1, 1),
    };

    private async Task<List<string>> PersonNames()
    {
        await using var context = new RetryingContext(_connection);

        return await context.Set<Person>().Select(x => x.Name).OrderBy(x => x).ToListAsync();
    }

    [Fact]
    public async Task A_transient_failure_is_retried_and_only_the_successful_attempt_survives()
    {
        await using var context = new RetryingContext(_connection);
        var unitOfWork = new UnitOfWork<RetryingContext>(context);
        var persons = new PersonRepository(context);
        var attempts = 0;

        await unitOfWork.ExecuteAsync(async token =>
        {
            attempts++;
            await persons.AddAsync(NewPerson($"Attempt {attempts}"), cancellationToken: token);

            if (attempts == 1) throw new TimeoutException("transient");
        });

        Assert.Equal(2, attempts);
        Assert.Equal(["Attempt 2"], await PersonNames());
    }

    [Fact]
    public async Task The_change_tracker_keeps_nothing_from_the_failed_attempt()
    {
        await using var context = new RetryingContext(_connection);
        var unitOfWork = new UnitOfWork<RetryingContext>(context);
        var persons = new PersonRepository(context);
        var attempts = 0;

        await unitOfWork.ExecuteAsync(async token =>
        {
            attempts++;
            await persons.AddAsync(NewPerson($"Attempt {attempts}"), cancellationToken: token);

            if (attempts == 1) throw new TimeoutException("transient");
        });

        // Ohne ChangeTracker.Clear() haenge hier die Entitaet des ersten Versuchs als Unchanged
        // im Context - der Context wuerde eine Zeile behaupten, die das Rollback entfernt hat.
        var tracked = context.ChangeTracker.Entries<Person>().Select(e => e.Entity.Name).ToList();

        Assert.Equal(["Attempt 2"], tracked);
    }

    [Fact]
    public async Task A_failure_that_is_not_transient_is_not_retried_and_rolls_back()
    {
        await using var context = new RetryingContext(_connection);
        var unitOfWork = new UnitOfWork<RetryingContext>(context);
        var persons = new PersonRepository(context);
        var attempts = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.ExecuteAsync(async token =>
        {
            attempts++;
            await persons.AddAsync(NewPerson("Never"), cancellationToken: token);

            throw new InvalidOperationException("permanent");
        }));

        Assert.Equal(1, attempts);
        Assert.Empty(await PersonNames());
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
