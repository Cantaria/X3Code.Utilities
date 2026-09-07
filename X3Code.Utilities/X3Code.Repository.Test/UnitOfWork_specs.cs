using Microsoft.EntityFrameworkCore;
using X3Code.Repository.Test.Configuration;
using X3Code.Repository.Test.Entities;
using X3Code.Repository.Test.Repositories;
using Xunit;

namespace X3Code.Repository.Test;

/// <summary>
/// What the unit of work has to guarantee: a block of writes arrives whole or not at all. Every test reads the
/// result back through a context of its own, after the scope has ended - that is the only moment at which the
/// question "did this reach the database" has an answer.
/// </summary>
public class UnitOfWork_specs
{
    [Fact]
    public async Task Completing_the_scope_makes_the_writes_visible()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var persons = new PersonRepository(context);

        await using (var unitOfWork = await new UnitOfWork(context).BeginAsync())
        {
            await persons.AddAsync(NewPerson("Kirk"));
            await unitOfWork.CompleteAsync();
        }

        Assert.Equal(1, await CountPersonsIn(database));
    }

    [Fact]
    public async Task Leaving_the_scope_without_completing_discards_the_writes()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var persons = new PersonRepository(context);

        await using (await new UnitOfWork(context).BeginAsync())
        {
            await persons.AddAsync(NewPerson("Kirk"));
            // No CompleteAsync - this is the path a forgotten or deliberately abandoned block takes.
        }

        Assert.Equal(0, await CountPersonsIn(database));
    }

    [Fact]
    public async Task An_exception_inside_the_scope_leaves_nothing_behind()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var persons = new PersonRepository(context);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await using var unitOfWork = await new UnitOfWork(context).BeginAsync();
            await persons.AddAsync(NewPerson("Kirk"));
            throw new InvalidOperationException("Something went wrong halfway through.");
        });

        Assert.Equal(0, await CountPersonsIn(database));
    }

    [Fact]
    public async Task Two_repositories_in_one_scope_are_one_unit()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var persons = new PersonRepository(context);
        var vehicleParts = new VehiclePartRepository(context);

        await using (await new UnitOfWork(context).BeginAsync())
        {
            await persons.AddAsync(NewPerson("Kirk"));
            await vehicleParts.AddAsync(new VehiclePart { EntityId = 1, Name = "Warp core", Price = 42.5m });
        }

        await using var reader = database.NewContext();
        Assert.Empty(reader.Set<Person>().AsNoTracking().ToList());
        Assert.Empty(reader.Set<VehiclePart>().AsNoTracking().ToList());
    }

    [Fact]
    public async Task A_scope_opened_inside_an_open_one_joins_it()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var persons = new PersonRepository(context);

        await using (var outer = await unitOfWork.BeginAsync())
        {
            var transactionOfTheOuterScope = context.Database.CurrentTransaction;

            await using (var inner = await unitOfWork.BeginAsync())
            {
                Assert.Same(transactionOfTheOuterScope, context.Database.CurrentTransaction);
                await persons.AddAsync(NewPerson("Kirk"));
                await inner.CompleteAsync();
            }

            // The inner scope said it was done, but it owns no transaction - nothing is committed yet.
            Assert.False(outer.IsCompleted);
            await outer.CompleteAsync();
        }

        Assert.Equal(1, await CountPersonsIn(database));
    }

    [Fact]
    public async Task An_inner_scope_left_without_completing_stops_the_outer_commit()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var persons = new PersonRepository(context);

        await using (var outer = await unitOfWork.BeginAsync())
        {
            await using (await unitOfWork.BeginAsync())
            {
                await persons.AddAsync(NewPerson("Kirk"));
            }

            await Assert.ThrowsAsync<InvalidOperationException>(() => outer.CompleteAsync());
        }

        Assert.Equal(0, await CountPersonsIn(database));
    }

    [Fact]
    public async Task Completing_a_scope_twice_is_refused()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();

        await using var unitOfWork = await new UnitOfWork(context).BeginAsync();
        await unitOfWork.CompleteAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.CompleteAsync());
    }

    private static Person NewPerson(string name)
    {
        return new Person { EntityId = Guid.NewGuid(), Name = name };
    }

    private static async Task<int> CountPersonsIn(TestDatabase database)
    {
        await using var context = database.NewContext();
        return await context.Set<Person>().AsNoTracking().CountAsync();
    }
}
