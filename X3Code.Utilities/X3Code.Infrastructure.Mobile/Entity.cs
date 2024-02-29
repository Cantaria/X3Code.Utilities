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
	public required string EntityId { get; set; }
}