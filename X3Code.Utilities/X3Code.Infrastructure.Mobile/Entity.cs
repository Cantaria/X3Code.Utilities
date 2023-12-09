using System;
using System.ComponentModel.DataAnnotations;

namespace X3Code.Infrastructure.Mobile;

/// <summary>
/// Represents an abstract entity class.
/// </summary>
public abstract class Entity : IEntity
{
	/// <summary>
	/// Gets or sets the identifier of the entity.
	/// </summary>
	/// <value>
	/// A string representing the entity identifier.
	/// </value>
	public string EntityId { get; set; }
}