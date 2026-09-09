using Microsoft.EntityFrameworkCore;

namespace X3Code.Repository;

/// <inheritdoc cref="IUnitOfWork"/>
/// <typeparam name="TContext">
/// The context the transaction is opened on. Naming it makes this resolvable from a container with
/// several contexts registered, and ties the unit of work to the same one the repositories use.
/// </typeparam>
public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    private readonly TContext _context;

    /// <summary>
    /// Takes the context the repositories are built on. It has to be the same instance they got, otherwise their
    /// writes go past the transaction this opens.
    /// </summary>
    /// <param name="context">The shared context.</param>
    public UnitOfWork(TContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    /// <summary>
    /// The scope that owns the open transaction, so a nested scope can reach it. It is cleared again when that
    /// scope is disposed - otherwise a later <see cref="BeginAsync"/> would hand a nested scope an outer one
    /// that is long gone, and the abandon report would go nowhere.
    /// </summary>
    private UnitOfWorkScope? _outermost;

    /// <inheritdoc/>
    public async Task<IUnitOfWorkScope> BeginAsync(CancellationToken cancellationToken = default)
    {
        // The documented guarantee is a database transaction. A provider that has none cannot give it, and
        // saying so here is better than handing out a scope that silently guarantees nothing.
        if (!_context.Database.IsRelational())
        {
            throw new InvalidOperationException(
                $"The provider [{_context.Database.ProviderName}] has no transactions, so a unit of work cannot guarantee anything. Use a relational provider for code that relies on it.");
        }

        var running = _context.Database.CurrentTransaction;

        // A transaction is already running - either one of ours further up the call stack, or one somebody
        // started on the context directly. Either way, opening a second one is not possible and not wanted:
        // the caller further out is the one that decides whether all of this is committed.
        if (running != null)
        {
            // Only report upwards if the running transaction really is the one our scope holds. If somebody
            // else started it, there is nobody of ours to tell.
            var owner = _outermost is not null && _outermost.Owns(running) ? _outermost : null;

            return new NestedUnitOfWorkScope(owner);
        }

        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _outermost = new UnitOfWorkScope(transaction, Release);

        return _outermost;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(Func<CancellationToken, Task> work, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);

        await ExecuteAsync<object?>(async token =>
        {
            await work(token);
            return null;
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> work, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);

        // The scope has to be opened inside the strategy: on a retry the transaction of the failed attempt is
        // gone and a new one has to be started. That is exactly what a caller-owned transaction cannot do,
        // and why EF refuses BeginTransaction under EnableRetryOnFailure.
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async token =>
        {
            await using var scope = await BeginAsync(token);

            var result = await work(token);

            await scope.CompleteAsync(token);

            return result;
        }, cancellationToken);
    }

    /// <summary>
    /// Called by the outermost scope when it is disposed. Guards against clearing a newer scope that was opened
    /// in the meantime.
    /// </summary>
    private void Release(UnitOfWorkScope scope)
    {
        if (ReferenceEquals(_outermost, scope))
        {
            _outermost = null;
        }
    }
}

/// <summary>
/// The unit of work without naming its context. Same behaviour as
/// <see cref="UnitOfWork{TContext}"/>, it just asks for the base <see cref="DbContext"/>.
/// </summary>
/// <remarks>
/// Kept for existing code. A container can only satisfy this when <see cref="DbContext"/> itself is
/// registered, which is ambiguous as soon as there is more than one context - name the context via
/// <see cref="UnitOfWork{TContext}"/> instead.
/// </remarks>
public class UnitOfWork : UnitOfWork<DbContext>
{
    /// <summary>
    /// Takes the context the repositories are built on.
    /// </summary>
    /// <param name="context">The shared context.</param>
    public UnitOfWork(DbContext context) : base(context)
    {
    }
}
