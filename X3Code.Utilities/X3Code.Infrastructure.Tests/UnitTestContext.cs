using Microsoft.EntityFrameworkCore;
using X3Code.Infrastructure.Tests.ConstValues;
using X3Code.Infrastructure.Tests.Data;

namespace X3Code.Infrastructure.Tests;

public class UnitTestContext : DbContext
{
    private readonly bool _emptyInitialized;

    #region Construction

    public UnitTestContext(DbContextOptions options) : base(options)
    {
    }

    public UnitTestContext()
    {
        _emptyInitialized = true;
    }

    #endregion

    #region Overrides Configuration

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured || _emptyInitialized)
        {
            optionsBuilder.UseSqlServer(DbConnectionStrings.GetConnectionString());
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Person>();
        entity.Property(x => x.Name).HasMaxLength(50);
        entity.Property(x => x.Surname).HasMaxLength(50);

        base.OnModelCreating(modelBuilder);
    }

    #endregion
}