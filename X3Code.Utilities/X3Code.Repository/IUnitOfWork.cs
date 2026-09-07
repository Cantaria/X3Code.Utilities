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
/// A provider that has no transactions - the in-memory one, for instance - cannot serve this, and
/// <see cref="BeginAsync"/> will say so rather than pretend the guarantee is there.
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Opens a unit of work. Nothing it contains reaches the database for good until
    /// <see cref="IUnitOfWorkScope.CompleteAsync"/> is called; disposing the scope without it discards everything.
    /// Opening one while another is still open joins the open one - only the outermost decides.
    /// </summary>
    Task<IUnitOfWorkScope> BeginAsync(CancellationToken cancellationToken = default);
}
