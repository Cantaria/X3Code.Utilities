using Xunit;

namespace X3Code.Infrastructure.Tests.Helper;

/// <summary>
/// Ties every test class that needs a database to the same <see cref="SqlServerFixture"/>,
/// so the container is started once per run instead of once per test class.
/// </summary>
[CollectionDefinition(Name)]
public class SqlServerCollection : ICollectionFixture<SqlServerFixture>
{
    /// <summary>
    /// Name to put on <see cref="CollectionAttribute"/> of the test classes.
    /// </summary>
    public const string Name = "SqlServer";
}
