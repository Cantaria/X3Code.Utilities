using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace X3Code.Repository;

/// <summary>
/// Registers the unit of work for a context.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IUnitOfWork{TContext}"/> - and, unless told otherwise, the untyped
    /// <see cref="IUnitOfWork"/> pointing at the same context.
    /// </summary>
    /// <typeparam name="TContext">The context the unit of work opens its transaction on. Register it with <c>AddDbContext</c> first.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">
    /// Has to match the lifetime of <typeparamref name="TContext"/>, which is scoped by default.
    /// A unit of work on a context of a different lifetime would open its transaction on a
    /// different instance than the repositories write on.
    /// </param>
    /// <param name="registerUntyped">
    /// Also register <see cref="IUnitOfWork"/> for this context. Leave it on with a single context.
    /// With several, only one of them can hold that registration - the last one wins - so turn it
    /// off and resolve <see cref="IUnitOfWork{TContext}"/> instead.
    /// </param>
    /// <returns>The service collection, for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDbContext&lt;AppContext&gt;(options =&gt; options.UseSqlServer(connectionString));
    /// services.AddX3CodeRepository&lt;AppContext&gt;();
    /// services.AddScoped&lt;IPersonRepository, PersonRepository&gt;();
    /// </code>
    /// where the repository names the context:
    /// <code>
    /// public class PersonRepository : BaseRepository&lt;Person, AppContext&gt;, IPersonRepository
    /// {
    ///     public PersonRepository(AppContext context) : base(context) { }
    /// }
    /// </code>
    /// </example>
    public static IServiceCollection AddX3CodeRepository<TContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        bool registerUntyped = true)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Add(new ServiceDescriptor(typeof(IUnitOfWork<TContext>), typeof(UnitOfWork<TContext>), lifetime));

        if (registerUntyped)
        {
            // Auf dieselbe Instanz zeigen, nicht eine zweite bauen: zwei Units of Work auf
            // demselben Context wuerden sich um dieselbe Transaktion streiten.
            services.Add(new ServiceDescriptor(
                typeof(IUnitOfWork),
                provider => provider.GetRequiredService<IUnitOfWork<TContext>>(),
                lifetime));
        }

        return services;
    }

    /// <summary>
    /// Registers <see cref="DbContext"/> as an alias for <typeparamref name="TContext"/>, so
    /// repositories deriving from the one-parameter <see cref="BaseRepository{TEntity}"/> can be
    /// resolved.
    /// </summary>
    /// <typeparam name="TContext">The context the alias points at.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">Has to match the lifetime of <typeparamref name="TContext"/>.</param>
    /// <returns>The service collection, for chaining.</returns>
    /// <remarks>
    /// Only for an application with exactly one context. With several, this alias is the very trap
    /// it looks like a convenience for: every repository asking for a bare <see cref="DbContext"/>
    /// silently lands on whichever context registered last, which may not be the one the unit of
    /// work opened its transaction on. Name the context on the repository instead - see
    /// <see cref="BaseRepository{TEntity, TContext}"/>.
    /// </remarks>
    public static IServiceCollection AddDbContextAlias<TContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Add(new ServiceDescriptor(
            typeof(DbContext),
            provider => provider.GetRequiredService<TContext>(),
            lifetime));

        return services;
    }
}
