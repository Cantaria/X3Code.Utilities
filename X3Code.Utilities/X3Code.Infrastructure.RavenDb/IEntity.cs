using System;

namespace X3Code.Infrastructure.RavenDb
{
    public interface IEntity
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        Guid EntityId { get; set; }

        /// <summary>
        /// RavenDb ID, will be automatically set
        /// </summary>
        string Id { get; set; }
    }
}