using System;

namespace X3Code.Infrastructure.RavenDb
{
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// RavenDb ID, will be automatically set
        /// </summary>
        public string Id { get; set; }
    }

}