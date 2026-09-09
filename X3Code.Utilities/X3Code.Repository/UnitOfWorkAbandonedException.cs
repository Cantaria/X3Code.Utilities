namespace X3Code.Repository;

/// <summary>
/// Thrown when a unit of work can no longer be committed because an inner scope was left without
/// completing it. Whoever opened that inner scope decided against its part, and committing the rest
/// would write exactly the half-finished state the unit of work exists to prevent.
/// </summary>
/// <remarks>
/// Derives from <see cref="InvalidOperationException"/>, which is what this used to be - existing
/// catch blocks keep working, and code that wants to tell this case apart from "already completed"
/// or "already disposed" now can.
/// </remarks>
public class UnitOfWorkAbandonedException : InvalidOperationException
{
    /// <summary>
    /// Creates the exception with the default message.
    /// </summary>
    public UnitOfWorkAbandonedException()
        : base("An inner unit of work was left without completing it, so this one cannot be committed.")
    {
    }

    /// <summary>
    /// Creates the exception with a specific message.
    /// </summary>
    /// <param name="message">The message describing the failure.</param>
    public UnitOfWorkAbandonedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates the exception with a specific message and an inner exception.
    /// </summary>
    /// <param name="message">The message describing the failure.</param>
    /// <param name="innerException">The exception that caused this one.</param>
    public UnitOfWorkAbandonedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
