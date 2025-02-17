using System.Diagnostics.CodeAnalysis;

namespace X3Code.Repository;

[Experimental("InProgress")]
public interface IEntity<T> where T : struct
{
    T EntityId { get; set; }
}