using System;
using X3Code.Infrastructure.Tests.Data;
using X3Code.Sql.Utils;
using X3Code.TestSupport;
using Xunit;

namespace X3Code.Infrastructure.Tests.Tests;

/// <summary>
/// Holds the interfaces of X3Code.Infrastructure against their implementations. IRepository.BulkWrite
/// kept two required, non-nullable parameters long after GenericRepository had made them optional -
/// the compiler does not match on parameter names, defaults or nullability, so nothing broke.
/// </summary>
/// <remarks>
/// Reflection only. This class deliberately carries no <see cref="CollectionAttribute"/>, so it
/// needs neither Docker nor the SQL Server container.
/// </remarks>
public class Contract_specs
{
    public static TheoryData<Type, Type> Contracts => new()
    {
        { typeof(IRepository<Person>), typeof(PersonRepository) },
        { typeof(IPersonRepository), typeof(PersonRepository) },
    };

    [Theory]
    [MemberData(nameof(Contracts))]
    public void Interface_and_implementation_agree(Type interfaceType, Type implementationType)
    {
        var drift = InterfaceContract.FindDrift(interfaceType, implementationType);

        Assert.True(drift.Count == 0, string.Join(Environment.NewLine, drift));
    }

    [Fact]
    public void The_sql_utils_connector_is_reachable_from_here()
    {
        // Haelt fest, dass DatabaseConnector wirklich aus X3Code.Sql.Utils kommt und nicht
        // versehentlich wieder in X3Code.Utilities zurueckwandert.
        Assert.Equal("X3Code.Sql.Utils", typeof(DatabaseConnector).Namespace);
        Assert.Equal("X3Code.Sql.Utils", typeof(DatabaseConnector).Assembly.GetName().Name);
    }
}
