using Microsoft.EntityFrameworkCore;

namespace X3Code.Repository;

/// <inheritdoc cref="IUnitOfWork"/>
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;

    /// <summary>
    /// Takes the context the repositories are built on. It has to be the same instance they got, otherwise their
    /// writes go past the transaction this opens.
    /// </summary>
    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Holds the scope that owns the open transaction, so a nested scope can reach it. It stays set after that
    /// scope is disposed, which does no harm: a disposed scope refuses everything anyway, and the next
    /// <see cref="BeginAsync"/> replaces it.
    /// </summary>
    private UnitOfWorkScope? _outermost;

    /// <inheritdoc/>
    public async Task<IUnitOfWorkScope> BeginAsync(CancellationToken cancellationToken = default)
    {
        // A transaction is already running - either one of ours further up the call stack, or one somebody
        // started on the context directly. Either way, opening a second one is not possible and not wanted:
        // the caller further out is the one that decides whether all of this is committed.
        if (_context.Database.CurrentTransaction != null)
        {
            return new NestedUnitOfWorkScope(_outermost);
        }

        var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        _outermost = new UnitOfWorkScope(transaction);

        return _outermost;
    }
}
