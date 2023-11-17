using System;

namespace X3Code.Infrastructure;

/// <summary>
/// Represents an entity in the system.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets or sets the identifier of an entity.
    /// </summary>
    /// <value>
    /// The unique identifier of the entity.
    /// </value>
    Guid EntityId { get; set; }
}