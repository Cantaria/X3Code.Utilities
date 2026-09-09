namespace X3Code.Repository;

/// <summary>
/// Opens a unit of work over the <see cref="Microsoft.EntityFrameworkCore.DbContext"/> the repositories share.
/// Every write the repositories do while a scope is open lands in one database transaction, so a sequence of
/// writes either arrives whole or not at all - without it, each single write is committed on its own and a
/// failure in the middle leaves the half of it that already went through.
/// </summary>
/// <remarks>
/// Register this with the same lifetime as the context (scoped in a web application). All repositories that are
/// meant to take part have to be built on that one context instance - a repository holding its own context
/// writes outside the transaction and is not covered.
/// <para>
/// A provider that has no transactions - the in-memory one, for instance - cannot serve this. <see cref="BeginAsync"/>
/// checks for that and throws rather than pretend the guarantee is there.
/// </para>
/// <para>
/// This is not thread-safe, and neither is the context it works on. One unit of work belongs to one logical
/// flow; do not open scopes from several threads over the same instance.
/// </para>
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Opens a unit of work. Nothing it contains reaches the database for good until
    /// <see cref="IUnitOfWorkScope.CompleteAsync"/> is called; disposing the scope without it discards everything.
    /// Opening one while another is still open joins the open one - only the outermost decides.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <exception cref="InvalidOperationException">The provider has no transactions.</exception>
    Task<IUnitOfWorkScope> BeginAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs <paramref name="work"/> inside a unit of work and commits it when the delegate returns without
    /// throwing. Use this instead of <see cref="BeginAsync"/> whenever the context may be configured with
    /// <c>EnableRetryOnFailure</c>: a retrying execution strategy refuses a transaction the caller started
    /// itself, so the whole block has to be handed to the strategy, which is what happens here.
    /// </summary>
    /// <param name="work">The writes that belong together. It may run more than once if the strategy retries, so keep it free of side effects outside the database.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <remarks>
    /// Before a retry the change tracker is cleared: the failed attempt was rolled back, but its
    /// entities would otherwise stay in the context - after a SaveChanges even as Unchanged, so the
    /// context would claim rows exist that the rollback took away. Do not rely on entities you
    /// tracked before the call still being tracked after it.
    /// <para>
    /// Do not call this inside an open scope. A retrying execution strategy refuses a transaction
    /// the caller started, and that is exactly what the surrounding scope holds -
    /// <see cref="BeginAsync"/> is the one that joins an open transaction.
    /// </para>
    /// </remarks>
    Task ExecuteAsync(Func<CancellationToken, Task> work, CancellationToken cancellationToken = default);

    /// <summary>
    /// Same as <see cref="ExecuteAsync(Func{CancellationToken, Task}, CancellationToken)"/>, for work that
    /// produces a result.
    /// </summary>
    /// <typeparam name="TResult">The type the work returns.</typeparam>
    /// <param name="work">The writes that belong together. It may run more than once if the strategy retries, so keep it free of side effects outside the database.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>Whatever <paramref name="work"/> returned on the attempt that succeeded.</returns>
    Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> work, CancellationToken cancellationToken = default);
}

/// <summary>
/// A unit of work over a named context. Resolve this instead of <see cref="IUnitOfWork"/> when the
/// application has more than one context - the untyped registration can only point at one of them.
/// </summary>
/// <typeparam name="TContext">The context this unit of work opens its transaction on.</typeparam>
public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : Microsoft.EntityFrameworkCore.DbContext
{
}
