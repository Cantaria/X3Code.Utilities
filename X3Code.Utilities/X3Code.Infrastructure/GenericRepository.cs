using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace X3Code.Infrastructure
{
    /// <summary>
    /// Provides basic database operations and capsulates the DbContext
    /// </summary>
    /// <typeparam name="TEntity">The Entitytype this repository is used</typeparam>
    /// <typeparam name="TContext">The underlying DbContext, which can be used for the TEntity</typeparam>
    public abstract class GenericRepository<TEntity, TContext> : IRepository<TEntity> where TEntity : class, IEntity, new() where TContext : DbContext
    {
        protected GenericRepository(TContext context)
        {
            DataBase = context;
            Entities = context.Set<TEntity>();
        }

        /// <summary>
        /// Direct access on the context
        /// </summary>
        protected TContext DataBase { get; }
        
        /// <summary>
        /// Direct access to the context TEntity
        /// </summary>
        protected DbSet<TEntity> Entities { get; }
        
        /// <summary>
        /// Search an return the entity
        /// </summary>
        /// <param name="predicate">The expression which should be used to search for an entity</param>
        /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
        /// <returns>The searched Entity, if found</returns>
        public TEntity Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return Entities.AsNoTracking().SingleOrDefault(predicate);
            }
            return Entities.SingleOrDefault(predicate);
        }

        /// <summary>
        /// Returns all entities for TEntity
        /// </summary>
        /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
        /// <returns>The searched Entity, if found</returns>
        public IEnumerable<TEntity> GetAll(bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return Entities.AsNoTracking().AsQueryable().ToList();   
            }
            return Entities.AsQueryable().ToList();
        }
        
        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return Entities.AsNoTracking().Where(predicate).ToList();    
            }
            return Entities.Where(predicate).ToList();
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return Entities.AsNoTracking().Where(predicate);
            }
            return Entities.Where(predicate);
        }

        public void Add(TEntity entity, bool asNoTracking = false)
        {
            Entities.Add(entity);
            DataBase.SaveChanges();

            if (asNoTracking)
            {
                RemoveFromTracking(entity);  
            }
        }

        public void AddAll(IEnumerable<TEntity> entities, bool asNoTracking = false)
        {
            var asList = entities.ToList();
            
            Entities.AddRange(asList);
            DataBase.SaveChanges();

            if (asNoTracking)
            {
                RemoveFromTracking(asList);
            }
        }

        public void Remove(TEntity entity, bool asNoTracking = false)
        {
            Entities.Remove(entity);
            DataBase.SaveChanges();

            if (asNoTracking)
            {
                RemoveFromTracking(entity);
            }
        }

        public void RemoveAll(IEnumerable<TEntity> entities, bool asNoTracking = false)
        {
            var asList = entities.ToList();
            
            Entities.RemoveRange(asList);
            DataBase.SaveChanges();
            
            if (asNoTracking)
            {
                RemoveFromTracking(asList.ToList());
            }
        }

        public void Update(TEntity entity, bool asNoTracking = false)
        {
            Entities.Update(entity);
            DataBase.SaveChanges();
            
            if (asNoTracking)
            {
                RemoveFromTracking(entity);
            }
        }

        public void UpdateAll(IEnumerable<TEntity> entities, bool asNoTracking = false)
        {
            var asList = entities.ToList();
            
            Entities.UpdateRange(asList);
            DataBase.SaveChanges();
            
            if (asNoTracking)
            {
                RemoveFromTracking(asList.ToList());
            }
        }

        /// <summary>
        /// Search an return the entity
        /// </summary>
        /// <param name="predicate">The expression which should be used to search for an entity</param>
        /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
        /// <returns>The searched Entity, if found</returns>
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return await Entities.AsNoTracking().SingleOrDefaultAsync(predicate);
            }
            return await Entities.SingleOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Returns all entities for TEntity
        /// </summary>
        /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
        /// <returns>The searched Entity, if found</returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return await Entities.AsNoTracking().ToListAsync();
            }
            return await Entities.ToListAsync();
        }

        public async Task AddAsync(TEntity entity, bool asNoTracking = false)
        {
            await Entities.AddAsync(entity);
            await DataBase.SaveChangesAsync();
            
            if (asNoTracking)
            {
                RemoveFromTracking(entity);
            }
        }

        public async Task AddAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false)
        {
            var asList = entities.ToList();
            
            await Entities.AddRangeAsync(asList);
            await DataBase.SaveChangesAsync();
            
            if (asNoTracking)
            {
                RemoveFromTracking(asList);
            }
        }

        public async Task RemoveAsync(TEntity entity, bool asNoTracking = false)
        {
            Entities.Remove(entity);
            await DataBase.SaveChangesAsync();
            
            if (asNoTracking)
            {
                RemoveFromTracking(entity);
            }
        }

        public async Task RemoveAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false)
        {
            var asList = entities.ToList();
            
            Entities.RemoveRange(asList);
            await DataBase.SaveChangesAsync();
            
            if (asNoTracking)
            {
                RemoveFromTracking(asList);
            }
        }

        public async Task UpdateAsync(TEntity entity, bool asNoTracking = false)
        {
            Entities.Update(entity);
            await DataBase.SaveChangesAsync();
            
            if (asNoTracking)
            {
                RemoveFromTracking(entity);
            }
        }

        public async Task UpdateAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false)
        {
            var asList = entities.ToList();
            
            Entities.UpdateRange(asList);
            await DataBase.SaveChangesAsync();
            
            if (asNoTracking)
            {
                RemoveFromTracking(asList.ToList());
            }
        }

        private void RemoveFromTracking(TEntity entity)
        {
            DataBase.Entry(entity).State = EntityState.Detached;
        }

        private void RemoveFromTracking(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DataBase.Entry(entity).State = EntityState.Detached;
            }
        }
    }
}
