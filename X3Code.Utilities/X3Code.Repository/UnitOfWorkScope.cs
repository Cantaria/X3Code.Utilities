using Microsoft.EntityFrameworkCore.Storage;

namespace X3Code.Repository;

/// <summary>
/// The scope that owns the transaction. The repositories keep calling SaveChanges as they always did - inside an
/// open transaction that only writes into it, and nothing becomes visible to anyone else until the commit.
/// That is why the existing repository methods did not have to change for this.
/// </summary>
internal sealed class UnitOfWorkScope : IUnitOfWorkScope
{
    private readonly IDbContextTransaction _transaction;
    private bool _isDisposed;
    private bool _isAbandoned;

    internal UnitOfWorkScope(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    /// <inheritdoc/>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Marks the unit of work as no longer committable. A nested scope calls this when it is left without being
    /// completed: whoever opened it decided against its part, and committing the rest would write exactly the
    /// half-finished state this whole thing exists to prevent.
    /// </summary>
    internal void Abandon()
    {
        _isAbandoned = true;
    }

    /// <inheritdoc/>
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (IsCompleted)
        {
            throw new InvalidOperationException("This unit of work was already completed.");
        }

        if (_isAbandoned)
        {
            throw new InvalidOperationException("An inner unit of work was left without completing it, so this one cannot be committed.");
        }

        await _transaction.CommitAsync(cancellationToken);
        IsCompleted = true;
    }

    /// <summary>
    /// Disposing an uncommitted transaction rolls it back - that is the ADO.NET contract Entity Framework passes
    /// on, and it is what makes "leaving the block discards the work" true without a rollback call of our own.
    /// Rolling back explicitly here would risk throwing while an exception is already on its way out and hiding
    /// the reason the block was left in the first place.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;

        _isDisposed = true;
        await _transaction.DisposeAsync();
    }
}
