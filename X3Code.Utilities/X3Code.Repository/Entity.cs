using System.Diagnostics.CodeAnalysis;

namespace X3Code.Repository;

[Experimental("InProgress")]
public class Entity<T> : IEntity<T> where T : struct
{
    public T EntityId { get; set; }
}