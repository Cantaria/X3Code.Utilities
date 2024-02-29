namespace X3Code.Infrastructure.Mobile;

/// <summary>
/// Represents an entity.
/// </summary>
public interface IEntity
{
	/// <summary>
	/// Gets or sets the identifier for an entity.
	/// </summary>
	/// <value>
	/// The entity identifier.
	/// </value>
	string EntityId { get; set; }
}