using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using X3Code.Infrastructure.Tests.ConstValues;

namespace X3Code.Infrastructure.Tests.Helper;

public class ContextFactory : IDesignTimeDbContextFactory<UnitTestContext>
{
    public UnitTestContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UnitTestContext>();
        optionsBuilder.UseSqlServer(DbConnectionStrings.GetConnectionString());

        return new UnitTestContext(optionsBuilder.Options);
    }
}