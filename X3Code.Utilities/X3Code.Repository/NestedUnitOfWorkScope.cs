using Microsoft.Extensions.Logging;

namespace X3Code.Repository;

/// <summary>
/// What a caller gets when it opens a unit of work inside one that is already running. It owns no transaction
/// and commits nothing - the outermost scope does that for everyone. Its one job is to report upwards when it
/// is left without being completed, so the outer scope knows it may not commit any more.
/// </summary>
internal sealed class NestedUnitOfWorkScope : IUnitOfWorkScope
{
    private readonly UnitOfWorkScope? _outermost;
    private readonly ILogger _logger;
    private bool _isDisposed;

    /// <param name="outermost">
    /// The scope holding the transaction, or null when it was started outside this unit of work and there is
    /// nobody to report to.
    /// </param>
    /// <param name="logger">The logger of the unit of work this scope belongs to.</param>
    internal NestedUnitOfWorkScope(UnitOfWorkScope? outermost, ILogger logger)
    {
        _outermost = outermost;
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool IsCompleted { get; private set; }

    /// <inheritdoc/>
    public bool IsAbandoned => _outermost?.IsAbandoned ?? false;

    /// <inheritdoc/>
    public Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (IsCompleted)
        {
            throw new InvalidOperationException("This unit of work was already completed.");
        }

        // A sibling scope was already left without completing, so the outer scope will never commit.
        // Reporting success here would promise something that cannot happen any more - the caller would
        // only find out at the outer CompleteAsync, far away from the code that is affected.
        if (IsAbandoned)
        {
            throw new UnitOfWorkAbandonedException(
                "The surrounding unit of work was already abandoned by another inner scope, so this one cannot be completed.");
        }

        IsCompleted = true;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        if (_isDisposed) return ValueTask.CompletedTask;

        _isDisposed = true;

        if (!IsCompleted)
        {
            // Ohne aeussere Scope gibt es niemanden zu benachrichtigen - dann wenigstens
            // festhalten, dass hier etwas unvollendet verlassen wurde.
            if (_outermost is null) UnitOfWorkLog.Discarded(_logger);
            else _outermost.Abandon();
        }

        return ValueTask.CompletedTask;
    }
}
