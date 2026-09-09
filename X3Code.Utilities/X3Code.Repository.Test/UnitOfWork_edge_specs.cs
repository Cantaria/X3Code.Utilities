using Microsoft.EntityFrameworkCore;
using X3Code.Repository.Test.Configuration;
using X3Code.Repository.Test.Context;
using X3Code.Repository.Test.Entities;
using X3Code.Repository.Test.Repositories;
using Xunit;

namespace X3Code.Repository.Test;

/// <summary>
/// The corners of the unit of work: what happens after the outer scope is gone, what a provider without
/// transactions gets told, how a sibling scope learns that the work is already lost, and the path for a
/// context configured with a retrying execution strategy.
/// </summary>
public class UnitOfWork_edge_specs
{
    private static Person NewPerson(string name) => new()
    {
        EntityId = Guid.NewGuid(),
        Name = name,
        Birthday = new DateTime(2000, 1, 1),
    };

    private static async Task<int> CountPersonsIn(TestDatabase database)
    {
        await using var context = database.NewContext();

        return await context.Set<Person>().CountAsync();
    }

    [Fact]
    public async Task A_second_scope_after_the_first_one_is_gone_is_a_fresh_unit_of_work()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var persons = new PersonRepository(context);

        await using (var first = await unitOfWork.BeginAsync())
        {
            await persons.AddAsync(NewPerson("First"));
            await first.CompleteAsync();
        }

        // Die zweite Scope muss wieder eine echte, eigene Transaktion aufmachen - nicht eine
        // Nested-Scope, die auf die laengst verworfene erste zeigt.
        await using (var second = await unitOfWork.BeginAsync())
        {
            Assert.False(second.IsCompleted);
            Assert.False(second.IsAbandoned);

            await persons.AddAsync(NewPerson("Second"));
            await second.CompleteAsync();
        }

        Assert.Equal(2, await CountPersonsIn(database));
    }

    [Fact]
    public async Task A_scope_opened_after_the_first_one_was_discarded_can_still_commit()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var persons = new PersonRepository(context);

        await using (await unitOfWork.BeginAsync())
        {
            await persons.AddAsync(NewPerson("Discarded"));
            // kein CompleteAsync
        }

        await using (var second = await unitOfWork.BeginAsync())
        {
            await persons.AddAsync(NewPerson("Kept"));
            await second.CompleteAsync();
        }

        Assert.Equal(1, await CountPersonsIn(database));
    }

    [Fact]
    public async Task A_transaction_started_outside_the_unit_of_work_is_joined_but_not_owned()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var persons = new PersonRepository(context);

        await using var foreign = await context.Database.BeginTransactionAsync();

        await using (var joined = await unitOfWork.BeginAsync())
        {
            await persons.AddAsync(NewPerson("Foreign"));
            // Kein CompleteAsync: es gibt keine eigene Scope, der das gemeldet werden koennte.
            Assert.False(joined.IsAbandoned);
        }

        await foreign.CommitAsync();

        Assert.Equal(1, await CountPersonsIn(database));
    }

    [Fact]
    public async Task A_scope_joining_a_foreign_transaction_does_not_report_to_a_scope_that_is_long_gone()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var persons = new PersonRepository(context);

        // Eine Unit of Work, die durch eine aufgegebene innere Scope unrettbar wurde und
        // dann verworfen wird. Frueher blieb genau diese Scope danach als "outermost" stehen.
        await using (var abandoned = await unitOfWork.BeginAsync())
        {
            await using (await unitOfWork.BeginAsync())
            {
                await persons.AddAsync(NewPerson("Lost"));
            }

            Assert.True(abandoned.IsAbandoned);
        }

        // Jetzt macht jemand anderes eine Transaktion auf dem Context auf.
        await using var foreign = await context.Database.BeginTransactionAsync();

        await using (var joined = await unitOfWork.BeginAsync())
        {
            // Diese Scope hat mit der verworfenen von oben nichts zu tun. Ohne Reset und
            // Ownership-Pruefung erbte sie deren Zustand und meldete faelschlich "aufgegeben".
            Assert.False(joined.IsAbandoned);

            await persons.AddAsync(NewPerson("Fresh"));
        }

        await foreign.CommitAsync();

        Assert.Equal(1, await CountPersonsIn(database));
    }

    [Fact]
    public async Task A_sibling_scope_learns_that_the_work_is_already_lost()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);

        await using var outer = await unitOfWork.BeginAsync();

        await using (await unitOfWork.BeginAsync())
        {
            // aufgegeben
        }

        await using var sibling = await unitOfWork.BeginAsync();

        Assert.True(sibling.IsAbandoned);
        // Vorher meldete das hier Erfolg, obwohl nichts davon je committet werden konnte.
        await Assert.ThrowsAsync<UnitOfWorkAbandonedException>(() => sibling.CompleteAsync());
    }

    [Fact]
    public async Task The_outer_scope_reports_that_it_is_abandoned()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);

        await using var outer = await unitOfWork.BeginAsync();
        Assert.False(outer.IsAbandoned);

        await using (await unitOfWork.BeginAsync())
        {
        }

        Assert.True(outer.IsAbandoned);
    }

    [Fact]
    public async Task A_provider_without_transactions_is_refused()
    {
        // X3AppContext konfiguriert den InMemory-Provider selbst.
        await using var context = new X3AppContext();
        var unitOfWork = new UnitOfWork(context);

        // Die Doku verspricht eine Datenbanktransaktion. Der InMemory-Provider hat keine, also
        // wird das gesagt statt eine Scope auszugeben, die nichts garantiert.
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.BeginAsync());
        Assert.Contains("no transactions", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_commits_when_the_work_returns()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var persons = new PersonRepository(context);

        await unitOfWork.ExecuteAsync(async token => await persons.AddAsync(NewPerson("Executed"), cancellationToken: token));

        Assert.Equal(1, await CountPersonsIn(database));
    }

    [Fact]
    public async Task ExecuteAsync_rolls_back_when_the_work_throws()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var persons = new PersonRepository(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.ExecuteAsync(async token =>
        {
            await persons.AddAsync(NewPerson("Rolled back"), cancellationToken: token);
            throw new InvalidOperationException("something went wrong");
        }));

        Assert.Equal(0, await CountPersonsIn(database));
    }

    [Fact]
    public async Task ExecuteAsync_hands_back_the_result_of_the_work()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var persons = new PersonRepository(context);

        var name = await unitOfWork.ExecuteAsync(async token =>
        {
            var person = NewPerson("Result");
            await persons.AddAsync(person, cancellationToken: token);

            return person.Name;
        });

        Assert.Equal("Result", name);
        Assert.Equal(1, await CountPersonsIn(database));
    }
}
