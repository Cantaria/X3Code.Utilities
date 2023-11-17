using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using X3Code.Infrastructure.Tests.ConstValues;
using X3Code.Infrastructure.Tests.Data;
using X3Code.Infrastructure.Tests.Helper;

namespace X3Code.Infrastructure.Tests.Tests;

public abstract class RepositoryBaseTest : IDisposable
{
    protected readonly IServiceProvider Services = BuildServiceProvider();
    protected readonly DatabaseConnector DataBaseConnector = new(DbConnectionStrings.GetConnectionString());
    
    protected const string PersonBaseQuery = "SELECT * FROM Person ";

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        var connectionString = DbConnectionStrings.GetConnectionString();
        services.AddDbContext<UnitTestContext>(options => { options.UseSqlServer(connectionString); });
        services.AddTransient<IPersonRepository, PersonRepository>();

        return services.BuildServiceProvider();
    }
    
    public void Dispose()
    {
        DataBaseConnector.CleanDataBase();
    }
}