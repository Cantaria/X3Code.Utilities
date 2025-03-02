using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace X3Code.Repository;

[Experimental("InProgress")]
public class Entity<T> : IEntity<T> where T : notnull
{
    [Key]
    public required T EntityId { get; set; }
}