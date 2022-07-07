using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace X3Code.Infrastructure
{
    public abstract class GenericRepository<TEntity, TContext> : IRepository<TEntity> where TEntity : class, IEntity, new() where TContext : DbContext
    {
		protected GenericRepository(TContext context)
        {
            DataBase = context;
            Entities = context.Set<TEntity>();
        }

        protected TContext DataBase { get; }
        protected DbSet<TEntity> Entities { get; }
        
        public TEntity Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return Entities.AsNoTracking().SingleOrDefault(predicate);
            }
            return Entities.SingleOrDefault(predicate);
        }

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
            Entities.RemoveRange(entities);
            DataBase.SaveChanges();
            
            if (asNoTracking)
            {
                RemoveFromTracking(entities.ToList());
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
            Entities.UpdateRange(entities);
            DataBase.SaveChanges();
            
            if (asNoTracking)
            {
                RemoveFromTracking(entities.ToList());
            }
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return await Entities.AsNoTracking().SingleOrDefaultAsync(predicate);
            }
            return await Entities.SingleOrDefaultAsync(predicate);
        }

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
            await Entities.AddRangeAsync(entities);
            await DataBase.SaveChangesAsync();
            
            if (asNoTracking)
            {
                RemoveFromTracking(entities.ToList());
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
            Entities.RemoveRange(entities);
            await DataBase.SaveChangesAsync();
            
            if (asNoTracking)
            {
                RemoveFromTracking(entities.ToList());
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
            Entities.UpdateRange(entities);
            await DataBase.SaveChangesAsync();
            
            if (asNoTracking)
            {
                RemoveFromTracking(entities.ToList());
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
