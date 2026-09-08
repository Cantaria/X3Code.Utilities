using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using X3Code.Infrastructure.Tests.Data;
using X3Code.Infrastructure.Tests.Helper;

namespace X3Code.Infrastructure.Tests.Tests;

public abstract class RepositoryBaseTest : IDisposable
{
    protected readonly IServiceProvider Services;
    protected readonly DatabaseConnector DataBaseConnector;

    protected const string PersonBaseQuery = "SELECT * FROM Person ";

    protected RepositoryBaseTest(SqlServerFixture database)
    {
        Services = BuildServiceProvider(database.ConnectionString);
        DataBaseConnector = new DatabaseConnector(database.ConnectionString);
    }

    private static IServiceProvider BuildServiceProvider(string connectionString)
    {
        var services = new ServiceCollection();

        services.AddDbContext<UnitTestContext>(options => { options.UseSqlServer(connectionString); });
        services.AddTransient<IPersonRepository, PersonRepository>();

        return services.BuildServiceProvider();
    }

    public void Dispose()
    {
        DataBaseConnector.CleanDataBase();
        GC.SuppressFinalize(this);
    }
}
