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
            return Entities.AsNoTracking();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
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
            throw new NotImplementedException();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task AddAllAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAllAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAllAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }
    }
}
