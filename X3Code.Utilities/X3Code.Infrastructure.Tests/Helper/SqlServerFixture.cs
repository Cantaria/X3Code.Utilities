using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace X3Code.Infrastructure.Tests.Helper;

/// <summary>
/// Starts one throwaway SQL Server in a Docker container for the whole test assembly and
/// applies the migrations against it. Replaces the hardcoded connection strings that used to
/// tie this project to specific developer machines.
/// </summary>
/// <remarks>
/// Requires a running Docker daemon. The container is removed again when the run finishes,
/// so the tests leave nothing behind and need no manual setup.
/// </remarks>
public class SqlServerFixture : IAsyncLifetime
{
    /// <summary>
    /// Image the container is built from. Pinned explicitly so local runs and CI use the
    /// same SQL Server version.
    /// </summary>
    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest";

    private readonly MsSqlContainer _container = new MsSqlBuilder(SqlServerImage).Build();

    /// <summary>
    /// Connection string to the running container. Only valid after <see cref="InitializeAsync"/>.
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// Starts the container and brings the schema up to the latest migration.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();

        var options = new DbContextOptionsBuilder<UnitTestContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        await using var context = new UnitTestContext(options);
        await context.Database.MigrateAsync();
    }

    /// <summary>
    /// Stops and removes the container.
    /// </summary>
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
