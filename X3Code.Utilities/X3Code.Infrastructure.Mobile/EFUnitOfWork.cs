using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace X3Code.Infrastructure.Mobile
{
	public class EFUnitOfWork<TContext> : IDisposable, IUnitOfWork where TContext : DbContext, new()
	{
		#region Fields

		private readonly string _connectionString;
		private TContext _context;
		private readonly object _syncContext = new object();

		#endregion

		#region Construction

		public EFUnitOfWork(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
                throw new ArgumentException("No ConnectionString given", nameof(connectionString));
			}
			_connectionString = connectionString;
		}

		private static TContext CreateInstance(string connectionString)
		{
			var classType = typeof(TContext);
			var classConstructor = classType.GetConstructor(new[] { typeof(DbContextOptions) });
			if (classConstructor == null)
			{
				var context = new TContext();
				return context;
			}

			var dbContextOptionBuilder = new DbContextOptionsBuilder();
			var builder = dbContextOptionBuilder.UseSqlite(connectionString);

			var result = (TContext)classConstructor.Invoke(new object[] { builder.Options });
			return result;
		}

		private TContext Context
		{
			get
			{
				lock (_syncContext)
				{
					if (_context == null)
					{
						_context = CreateInstance(_connectionString);
					}

					return _context;
				}
			}
		}

		#endregion

		public void Dispose()
		{
			if (_context == null) return;

			lock (_syncContext)
			{
				if (_context != null)
				{
					_context.Dispose();
					_context = null;
				}
			}
		}

		public IQueryable<T> Query<T>() where T : class, new()
		{
			return Context.Set<T>();
		}

        public void Remove<T>(T entity) where T : class, new()
		{
			var entry = Context.Entry(entity);
			if (entry != null)
			{
				entry.State = EntityState.Deleted;
			}
			else
			{
				Context.Set<T>().Attach(entity);
			}
			Context.Entry(entity).State = EntityState.Deleted;
			SaveChanges();
		}

		public async Task DeleteAsync<T>(T entity) where T : class, new()
		{
			var entry = Context.Entry(entity);
			if (entry != null)
			{
				entry.State = EntityState.Deleted;
			}
			else
			{
				Context.Set<T>().Attach(entity);
			}
			Context.Entry(entity).State = EntityState.Deleted;
			await SaveChangesAsync();
		}

		public void AddRange<T>(IEnumerable<T> entities) where T : class, new()
		{
			Context.Set<T>().AddRange(entities);
			SaveChanges();
		}

		public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, new()
		{
			await Context.Set<T>().AddRangeAsync(entities);
			await SaveChangesAsync();
		}

		public void RemoveRange<T>(IEnumerable<T> entities) where T : class, new()
		{
			Context.Set<T>().RemoveRange(entities);
			SaveChanges();
		}

		public async Task RemoveRangeAsync<T>(IEnumerable<T> entities) where T : class, new()
		{
			Context.Set<T>().RemoveRange(entities);
			await SaveChangesAsync();
		}

		public void RemoveRange<T>(Func<T, bool> predicate) where T : class, new()
		{
			var entities = Context.Set<T>().Where(predicate).ToList();
			Context.Set<T>().RemoveRange(entities);
			SaveChanges();
		}

		public async Task RemoveRangeAsync<T>(Func<T, bool> predicate) where T : class, new()
		{
			var entities = Context.Set<T>().Where(predicate).ToList();
			Context.Set<T>().RemoveRange(entities);
			await SaveChangesAsync();
		}

		public void Complete()
	    {
	        Dispose();
	    }

		public void Add<T>(T entity) where T : class, IEntity, new()
		{
            var entityInContext = Context.Set<T>().SingleOrDefault(x => x.EntityId == entity.EntityId);

			if (entityInContext != null)
				Update(entityInContext, entity);
			else
				Insert(entity);

			SaveChanges();
		}

		public async Task AddAsync<T>(T entity) where T : class, IEntity, new()
		{
			var entityInContext = await Context.Set<T>().SingleOrDefaultAsync(x => x.EntityId == entity.EntityId);

			if (entityInContext != null)
				Update(entityInContext, entity);
			else
				await InsertAsync(entity);

			await SaveChangesAsync();
		}

		private void SaveChanges()
		{
			try
			{
				Context.SaveChanges();
			}
			catch (Exception ex)
			{
                try
				{
					_context = CreateInstance(_connectionString);
				}
				catch(Exception innerEx)
				{
					throw new Exception($"SaveChange() failed: [{ex.Message}]", innerEx);
				}
				throw;
			}
		}

		private async Task SaveChangesAsync()
		{
			try
			{
				await Context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
                try
				{
					_context = CreateInstance(_connectionString);
				}
				catch (Exception innerEx)
				{
					throw new Exception($"SaveChangesAsync() failed: [{ex.Message}]", innerEx);
				}
				throw;
			}
		}

		private void Insert<T>(T entity) where T : class, new()
		{
			Context.Set<T>().Add(entity);
		}

		private async Task InsertAsync<T>(T entity) where T : class, new()
		{
			await Context.Set<T>().AddAsync(entity);
		}

		private void Update<T>(T fromContext, T entity) where T : class, IEntity, new()
		{
			var attachedEntry = Context.Entry(fromContext);
			attachedEntry.CurrentValues.SetValues(entity);

			attachedEntry.State = EntityState.Modified;
		}
	}

}