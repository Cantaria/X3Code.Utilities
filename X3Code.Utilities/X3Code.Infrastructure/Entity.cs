using System;
using System.ComponentModel.DataAnnotations;

namespace X3Code.Infrastructure;

/// <summary>
/// This class represents an abstract entity in the application.
/// </summary>
/// <remarks>
/// An entity is an object in the application domain that has an identity and can be persisted
/// to a data store. This abstract class provides a base implementation for entities,
/// including the EntityId property which serves as the unique identifier for each entity.
/// </remarks>
public abstract class Entity : IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    [Key]
    public abstract Guid EntityId { get; set; }
}