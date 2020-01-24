using System;

namespace X3Code.Infrastructure
{
    public interface IEntity
    {
        Guid EntityId { get; set; }
    }
}
