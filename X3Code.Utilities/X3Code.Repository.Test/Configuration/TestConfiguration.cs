using Microsoft.Extensions.DependencyInjection;
using X3Code.Repository.Test.Context;
using X3Code.Repository.Test.Repositories;

namespace X3Code.Repository.Test.Configuration;

internal static class TestConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        //context
        services.AddDbContext<X3AppContext>();
        services.AddTransient<IPersonRepository, PersonRepository>();
        services.AddTransient<IVehiclePartRepository, VehiclePartRepository>();
    }
}