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
        
        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.SingleOrDefault(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return Entities.AsQueryable();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.Where(predicate);
        }
        
        public IQueryable<TEntity> GetAllAsNoTracking()
        {
            return Entities.AsNoTracking();
        }

        public IQueryable<TEntity> GetAllAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return Entities.AsNoTracking().Where(predicate);
        }

        public void Add(TEntity entity)
        {
            Entities.Add(entity);
            DataBase.SaveChanges();
        }

        public void AddAll(IEnumerable<TEntity> entities)
        {
            Entities.AddRange(entities);
            DataBase.SaveChanges();
        }

        public void Remove(TEntity entity)
        {
            Entities.Remove(entity);
            DataBase.SaveChanges();
        }

        public void RemoveAll(IEnumerable<TEntity> entities)
        {
            Entities.RemoveRange(entities);
            DataBase.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            Entities.Update(entity);
            DataBase.SaveChanges();
        }

        public void UpdateAll(IEnumerable<TEntity> entities)
        {
            Entities.UpdateRange(entities);
            DataBase.SaveChanges();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Entities.FindAsync(predicate);
        }

        public async Task<IQueryable<TEntity>> GetAllAsync()
        {
            return await Task.Run(() => Entities.AsNoTracking());
        }

        public async Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.Run(() => Entities.AsNoTracking().Where(predicate));
        }

        public async Task AddAsync(TEntity entity)
        {
            await Entities.AddAsync(entity);
        }

        public async Task AddAllAsync(IEnumerable<TEntity> entities)
        {
            await Entities.AddRangeAsync(entities);
        }

        public async Task RemoveAsync(TEntity entity)
        {
            await Task.Run(() => Entities.Remove(entity));
        }

        public async Task RemoveAllAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => Entities.RemoveRange(entities));
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() => Entities.Update(entity));
        }

        public async Task UpdateAllAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => Entities.UpdateRange(entities));
        }
    }
}
