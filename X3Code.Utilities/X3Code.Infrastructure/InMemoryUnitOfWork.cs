using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace X3Code.Infrastructure
{
    public class InMemoryUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly Dictionary<Type, HashSet<object>> _entitiesByType = new Dictionary<Type, HashSet<object>>();

        protected HashSet<object> this[Type key]
        {
            get
            {
                if (_entitiesByType.ContainsKey(key))
                {
                    return _entitiesByType[key];
                }
                else
                {
                    var emptyList = new HashSet<object>();
                    _entitiesByType.Add(key, emptyList);
                    return emptyList;
                }
            }
        }

		public void Dispose()
        {
            _entitiesByType.Clear();
        }

		public IQueryable<TEntity> Query<TEntity>() where TEntity : class, new()
        {
            var listOfObjects = this[typeof(TEntity)];
            var typedListEntites = new List<TEntity>();
            foreach (var untypedObject in listOfObjects)
            {
                var typedObject = untypedObject as TEntity;
                if (typedObject == null)
                {
                    throw new InvalidOperationException($"Conversion from [{untypedObject.GetType()}] to [{typeof(TEntity)}] failed.");
                }
				typedListEntites.Add(typedObject);
            }

            return typedListEntites.AsQueryable();
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class, new()
        {
            this[typeof(TEntity)].Remove(entity);
        }

		public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, new()
		{
			this[typeof(TEntity)].Remove(entity);
		}

		public void Add<TEntity>(TEntity entity) where TEntity : class, IEntity, new()
        {
            var listOfEntities = this[typeof(TEntity)];
            if (listOfEntities.Contains(entity))
            {
                return;
            }
            listOfEntities.Add(entity);
        }

		public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class, IEntity, new()
		{
			var listOfEntities = this[typeof(TEntity)];
			if (listOfEntities.Contains(entity))
			{
				return;
			}
			listOfEntities.Add(entity);
		}

		public void AddRange<TEntity>(IEnumerable<TEntity> listOfEntities) where TEntity : class, new()
        {
            var storage = this[typeof(TEntity)];
            foreach (var entityToStore in listOfEntities)
            {
                if (storage.Contains(entityToStore)) continue;
                storage.Add(entityToStore);
            }
        }

		public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> listOfEntities) where TEntity : class, new()
		{
			var storage = this[typeof(TEntity)];
			foreach (var entityToStore in listOfEntities)
			{
				if (storage.Contains(entityToStore)) continue;
				storage.Add(entityToStore);
			}
		}

		public void RemoveRange<TEntity>(IEnumerable<TEntity> listOfEntities) where TEntity : class, new()
        {
            var storage = this[typeof(TEntity)];
            foreach (var entityToRemove in listOfEntities)
            {
                storage.Remove(entityToRemove);
            }
        }

		public async Task RemoveRangeAsync<TEntity>(IEnumerable<TEntity> listOfEntities) where TEntity : class, new()
		{
			var storage = this[typeof(TEntity)];
			foreach (var entityToRemove in listOfEntities)
			{
				storage.Remove(entityToRemove);
			}
		}

		public void RemoveRange<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, new()
        {
            var entitiesToRemove = Query<TEntity>().AsEnumerable().Where(predicate);
            RemoveRange(entitiesToRemove);
        }

		public async Task RemoveRangeAsync<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, new()
		{
			var entitiesToRemove = Query<TEntity>().AsEnumerable().Where(predicate);
			await RemoveRangeAsync(entitiesToRemove);
		}

		public void Complete()
        {
        }
	}
}
