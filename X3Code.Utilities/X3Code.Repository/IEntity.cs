namespace X3Code.Repository;

public interface IEntity<T> where T : notnull
{
    T EntityId { get; set; }
}