using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using X3Code.Repository.Test.Entities;
using Xunit;

namespace X3Code.Repository.Test;

/// <summary>
/// The repository and the unit of work used to ask for the base DbContext. A container with more
/// than one context cannot satisfy that, and registering DbContext by hand to get around it is the
/// very thing that lets a repository end up on a different context than the unit of work that
/// opened the transaction.
/// </summary>
public class DependencyInjection_specs : IDisposable
{
    private readonly SqliteConnection _first = new("DataSource=:memory:");
    private readonly SqliteConnection _second = new("DataSource=:memory:");

    public DependencyInjection_specs()
    {
        _first.Open();
        _second.Open();
    }

    #region Two contexts over the same entities

    public class FirstContext : DbContext
    {
        public FirstContext(DbContextOptions<FirstContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Person>().HasKey(x => x.EntityId);
    }

    public class SecondContext : DbContext
    {
        public SecondContext(DbContextOptions<SecondContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Person>().HasKey(x => x.EntityId);
    }

    public interface IFirstPersonRepository : IBaseRepository<Person>;

    public class FirstPersonRepository : BaseRepository<Person, FirstContext>, IFirstPersonRepository
    {
        public FirstPersonRepository(FirstContext context) : base(context) { }
    }

    #endregion

    private ServiceProvider BuildProvider(bool registerUntyped = false)
    {
        var services = new ServiceCollection();

        services.AddDbContext<FirstContext>(options => options.UseSqlite(_first));
        services.AddDbContext<SecondContext>(options => options.UseSqlite(_second));

        services.AddX3CodeRepository<FirstContext>(registerUntyped: registerUntyped);
        services.AddX3CodeRepository<SecondContext>(registerUntyped: false);
        services.AddScoped<IFirstPersonRepository, FirstPersonRepository>();

        return services.BuildServiceProvider();
    }

    [Fact]
    public void A_repository_naming_its_context_resolves_next_to_a_second_context()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        // Ohne den Kontext-Typparameter waere das hier nicht aufloesbar - der Konstruktor
        // haette nach dem Basistyp DbContext gefragt, den niemand registriert hat.
        var repository = scope.ServiceProvider.GetRequiredService<IFirstPersonRepository>();

        Assert.NotNull(repository);
    }

    [Fact]
    public void Both_units_of_work_resolve_and_stay_apart()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var first = scope.ServiceProvider.GetRequiredService<IUnitOfWork<FirstContext>>();
        var second = scope.ServiceProvider.GetRequiredService<IUnitOfWork<SecondContext>>();

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.NotSame(first, second);
    }

    [Fact]
    public async Task The_unit_of_work_and_the_repository_share_one_context()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<FirstContext>();
        await context.Database.EnsureCreatedAsync();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<FirstContext>>();
        var repository = scope.ServiceProvider.GetRequiredService<IFirstPersonRepository>();

        await using (await unitOfWork.BeginAsync())
        {
            await repository.AddAsync(new Person { EntityId = Guid.NewGuid(), Name = "Shared" });
            // kein CompleteAsync
        }

        // Nur wenn beide auf derselben Instanz sitzen, nimmt das Rollback den Schreibvorgang mit.
        await using var verification = new FirstContext(
            new DbContextOptionsBuilder<FirstContext>().UseSqlite(_first).Options);

        Assert.Equal(0, await verification.Set<Person>().CountAsync());
    }

    [Fact]
    public void The_untyped_unit_of_work_points_at_the_same_instance_as_the_typed_one()
    {
        using var provider = BuildProvider(registerUntyped: true);
        using var scope = provider.CreateScope();

        var typed = scope.ServiceProvider.GetRequiredService<IUnitOfWork<FirstContext>>();
        var untyped = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Zwei getrennte Instanzen wuerden sich um dieselbe Transaktion streiten.
        Assert.Same(typed, untyped);
    }

    [Fact]
    public void Without_the_untyped_registration_there_is_no_ambiguous_IUnitOfWork()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        Assert.Null(scope.ServiceProvider.GetService<IUnitOfWork>());
    }

    [Fact]
    public void The_DbContext_alias_makes_the_untyped_repository_resolvable()
    {
        var services = new ServiceCollection();
        services.AddDbContext<FirstContext>(options => options.UseSqlite(_first));
        services.AddX3CodeRepository<FirstContext>();
        services.AddDbContextAlias<FirstContext>();
        services.AddScoped<BaseRepository<Person>>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<BaseRepository<Person>>();

        Assert.NotNull(repository);
    }

    public void Dispose()
    {
        _first.Dispose();
        _second.Dispose();
        GC.SuppressFinalize(this);
    }
}
