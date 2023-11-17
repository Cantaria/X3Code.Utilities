using System;
using System.ComponentModel.DataAnnotations;

namespace X3Code.Infrastructure;

public abstract class Entity : IEntity
{
    [Key]
    public abstract Guid EntityId { get; set; }
}