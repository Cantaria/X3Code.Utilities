namespace X3Code.Repository;

/// <summary>
/// What a caller gets when it opens a unit of work inside one that is already running. It owns no transaction
/// and commits nothing - the outermost scope does that for everyone. Its one job is to report upwards when it
/// is left without being completed, so the outer scope knows it may not commit any more.
/// </summary>
internal sealed class NestedUnitOfWorkScope : IUnitOfWorkScope
{
    private readonly UnitOfWorkScope? _outermost;
    private bool _isDisposed;

    /// <param name="outermost">
    /// The scope holding the transaction, or null when it was started outside this unit of work and there is
    /// nobody to report to.
    /// </param>
    internal NestedUnitOfWorkScope(UnitOfWorkScope? outermost)
    {
        _outermost = outermost;
    }

    /// <inheritdoc/>
    public bool IsCompleted { get; private set; }

    /// <inheritdoc/>
    public Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (IsCompleted)
        {
            throw new InvalidOperationException("This unit of work was already completed.");
        }

        IsCompleted = true;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        if (_isDisposed) return ValueTask.CompletedTask;

        _isDisposed = true;
        if (!IsCompleted) _outermost?.Abandon();

        return ValueTask.CompletedTask;
    }
}
