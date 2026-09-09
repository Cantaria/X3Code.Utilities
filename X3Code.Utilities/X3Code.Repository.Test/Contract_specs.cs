using X3Code.Repository.Test.Configuration;
using X3Code.Repository.Test.Context;
using X3Code.Repository.Test.Entities;
using X3Code.TestSupport;
using Xunit;

namespace X3Code.Repository.Test;

/// <summary>
/// Holds the interfaces of X3Code.Repository against their implementations. Parameter names,
/// default values and nullability are not part of the signature the compiler matches on, so an
/// interface can drift away from its implementation without anything failing to build.
/// </summary>
public class Contract_specs
{
    public static TheoryData<Type, Type> Contracts => new()
    {
        { typeof(IBaseRepository<Person>), typeof(BaseRepository<Person, TransactionalContext>) },
        { typeof(IBaseRepository<Person>), typeof(BaseRepository<Person>) },
        { typeof(IUnitOfWork), typeof(UnitOfWork<TransactionalContext>) },
        { typeof(IUnitOfWork), typeof(UnitOfWork) },
        { typeof(IUnitOfWork<TransactionalContext>), typeof(UnitOfWork<TransactionalContext>) },
    };

    [Theory]
    [MemberData(nameof(Contracts))]
    public void Interface_and_implementation_agree(Type interfaceType, Type implementationType)
    {
        var drift = InterfaceContract.FindDrift(interfaceType, implementationType);

        Assert.True(drift.Count == 0, string.Join(Environment.NewLine, drift));
    }

    [Fact]
    public async Task Calling_the_repository_through_its_interface_with_named_arguments_works()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();

        IBaseRepository<Person> repository = new BaseRepository<Person, TransactionalContext>(context);

        var person = new Person { EntityId = Guid.NewGuid(), Name = "Contract", Birthday = new DateTime(2000, 1, 1) };
        await repository.AddAsync(person, asNoTracking: false);

        var found = await repository.GetAsync(x => x.Name == "Contract", asNoTracking: true);

        Assert.NotNull(found);
        Assert.Equal(person.EntityId, found.EntityId);
    }

    [Fact]
    public async Task Bulk_writing_through_the_interface_needs_no_table_name()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();

        IBaseRepository<Person> repository = new BaseRepository<Person, TransactionalContext>(context);

        // Ueber die Schnittstelle aufgerufen, ohne tableName - genau die Zusage, die bei
        // X3Code.Infrastructure auf dem Interface fehlte, waehrend die Klasse sie schon hielt.
        var written = await repository.BulkWrite([
            new Person { EntityId = Guid.NewGuid(), Name = "Bulk", Birthday = new DateTime(2000, 1, 1) },
        ]);

        Assert.Equal(1, written);
    }
}
