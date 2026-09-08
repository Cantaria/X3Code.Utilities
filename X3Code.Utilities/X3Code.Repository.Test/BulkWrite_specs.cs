using Xunit;
using Microsoft.EntityFrameworkCore;
using X3Code.Repository.Test.Configuration;
using X3Code.Repository.Test.Entities;
using X3Code.Repository.Test.Repositories;

namespace X3Code.Repository.Test;

/// <summary>
/// BulkWrite used to take a tableName and a timeOutInSeconds and read neither of them. The
/// table name is gone - Entity Framework writes to the table the entity is mapped to - and the
/// timeout is applied to the context now.
/// </summary>
public class BulkWrite_specs
{
    private static IEnumerable<Person> CreatePersons(int count) =>
        Enumerable.Range(0, count).Select(i => new Person
        {
            EntityId = Guid.NewGuid(),
            Name = $"Person {i}",
            Birthday = new DateTime(2000, 1, 1),
        });

    [Fact]
    public async Task Writes_every_entity_across_several_batches()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var repository = new PersonRepository(context);

        var written = await repository.BulkWrite(CreatePersons(2500), batchSize: 1000);

        Assert.Equal(2500, written);
        Assert.Equal(2500, await context.Set<Person>().CountAsync());
    }

    [Fact]
    public async Task Writes_nothing_and_returns_zero_for_an_empty_collection()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var repository = new PersonRepository(context);

        var written = await repository.BulkWrite([]);

        Assert.Equal(0, written);
        Assert.Equal(0, await context.Set<Person>().CountAsync());
    }

    [Fact]
    public async Task Restores_the_command_timeout_afterwards()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        context.Database.SetCommandTimeout(42);
        var repository = new PersonRepository(context);

        await repository.BulkWrite(CreatePersons(5), timeOutInSeconds: 120);

        Assert.Equal(42, context.Database.GetCommandTimeout());
    }

    [Fact]
    public async Task Rejects_a_batch_size_that_is_not_positive()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var repository = new PersonRepository(context);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => repository.BulkWrite(CreatePersons(5), batchSize: 0));
    }

    [Fact]
    public async Task Leaves_written_entities_untracked()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var repository = new PersonRepository(context);

        await repository.BulkWrite(CreatePersons(10));

        Assert.Empty(context.ChangeTracker.Entries<Person>());
    }

    [Fact]
    public async Task Is_rolled_back_as_a_whole_inside_a_unit_of_work()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork(context);
        var repository = new PersonRepository(context);

        await using (await unitOfWork.BeginAsync())
        {
            await repository.BulkWrite(CreatePersons(2500), batchSize: 1000);
            // kein CompleteAsync - die Scope faellt auseinander und rollt zurueck
        }

        await using var verification = database.NewContext();
        Assert.Equal(0, await verification.Set<Person>().CountAsync());
    }
}
