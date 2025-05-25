using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace X3Code.Repository;

/// <summary>
/// Represents a generic entity with a primary key of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// The type of the primary key for the entity. Must be non-nullable.
/// </typeparam>
[Experimental("InProgress")]
public class Entity<T> : IEntity<T> where T : notnull
{
    /// <summary>
    /// Represents the unique identifier for an entity.
    /// </summary>
    /// <typeparam name="T">The data type of the entity identifier, which must be non-nullable.</typeparam>
    /// <remarks>
    /// This property is marked as a required key and is used as the primary identifier for instances of the entity.
    /// It adheres to constraints defined in the database schema and is integral for entity tracking and database operations.
    /// </remarks>
    [Key]
    public required T EntityId { get; set; }
}