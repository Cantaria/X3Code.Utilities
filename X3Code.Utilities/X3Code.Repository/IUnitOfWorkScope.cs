namespace X3Code.Repository;

/// <summary>
/// One open unit of work. It is meant to be held in an <c>await using</c> and finished with
/// <see cref="CompleteAsync"/>:
/// <code>
/// await using var unitOfWork = await _unitOfWork.BeginAsync();
/// await _personRepository.AddAsync(person);
/// await _vehiclePartRepository.UpdateAsync(part);
/// await unitOfWork.CompleteAsync();
/// </code>
/// Leaving the block without that call - by returning early, or because something was thrown - rolls everything
/// back. Completing is the deliberate act, discarding is what happens by itself, so a path never
/// thought through cannot commit half a change.
/// </summary>
public interface IUnitOfWorkScope : IAsyncDisposable
{
    /// <summary>
    /// True, once <see cref="CompleteAsync"/> has run through. Until then, disposing throws the workaway.
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// True, once an inner scope was left without completing it. From that point on the unit of work can only
    /// be rolled back, and <see cref="CompleteAsync"/> throws a <see cref="UnitOfWorkAbandonedException"/>.
    /// Check it to leave early instead of doing more work that cannot be committed anyway.
    /// </summary>
    bool IsAbandoned { get; }

    /// <summary>
    /// Commits everything that was written since the scope was opened.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <exception cref="InvalidOperationException">The scope was already completed.</exception>
    /// <exception cref="UnitOfWorkAbandonedException">An inner scope was left without completing it.</exception>
    /// <exception cref="ObjectDisposedException">The scope was already disposed of.</exception>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
