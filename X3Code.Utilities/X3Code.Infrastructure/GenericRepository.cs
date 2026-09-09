using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace X3Code.Infrastructure;

/// <summary>
/// Provides basic database operations and encapsulates the DbContext
/// </summary>
/// <typeparam name="TEntity">The Entity type this repository is used</typeparam>
/// <typeparam name="TContext">The underlying DbContext, which can be used for the TEntity</typeparam>
public abstract class GenericRepository<TEntity, TContext> : IRepository<TEntity>
    where TEntity : class, IEntity where TContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{TEntity,TContext}"/> class.
    /// </summary>
    /// <param name="context">The context used to access the database.</param>
    protected GenericRepository(TContext context)
    {
        DataBase = context;
        Entities = context.Set<TEntity>();
    }

    /// <summary>
    /// Direct access to the context
    /// </summary>
    protected TContext DataBase { get; }

    /// <summary>
    /// Direct access to the context TEntity
    /// </summary>
    protected DbSet<TEntity> Entities { get; }

    #region Entity Framework database operations

    /// <summary>
    /// Search and return the entity
    /// </summary>
    /// <param name="predicate">The expression that should be used to search for an entity</param>
    /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
    /// <returns>The searched Entity, if found</returns>
    public TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
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

    /// <summary>
    /// Returns all entities matching the given predicate as IEnumerable
    /// </summary>
    /// <param name="predicate">The predicate for selecting entities</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <returns></returns>
    public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().Where(predicate).ToList();
        }

        return Entities.Where(predicate).ToList();
    }

    /// <summary>
    /// Searches for the corresponding entities that match the predicate
    /// </summary>
    /// <param name="predicate">The predicate for selecting entities</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <returns></returns>
    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().Where(predicate).AsQueryable();
        }

        return Entities.Where(predicate).AsQueryable();
    }

    /// <summary>
    /// Searches for the corresponding entities that match the predicate
    /// </summary>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <returns></returns>
    public IQueryable<TEntity> Query(bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return Entities.AsNoTracking().AsQueryable();
        }

        return Entities.AsQueryable();
    }

    /// <summary>
    /// Adds the entity to the database
    /// </summary>
    /// <param name="entity">The entity that should be saved in a database</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    public void Add(TEntity entity, bool asNoTracking = false)
    {
        Entities.Add(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Adds all entities to the database
    /// </summary>
    /// <param name="entities">Entities that should be saved into a database</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
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

    /// <summary>
    /// Removes the given entity from the database
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    public void Remove(TEntity entity, bool asNoTracking = false)
    {
        Entities.Remove(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Removes all given entities from the database
    /// </summary>
    /// <param name="entities">The entities to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
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

    /// <summary>
    /// Updates the given entity from a database
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    public void Update(TEntity entity, bool asNoTracking = false)
    {
        Entities.Update(entity);
        DataBase.SaveChanges();

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Updates all given entities from the database
    /// </summary>
    /// <param name="entities">The entities to update</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
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
    /// Search and return the entity
    /// </summary>
    /// <param name="predicate">The expression that should be used to search for an entity</param>
    /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
    /// <returns>The searched Entity, if found</returns>
    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().SingleOrDefaultAsync(predicate, cancellationToken);
        }

        return await Entities.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Returns all entities for TEntity
    /// </summary>
    /// <param name="asNoTracking">Optional: Do not track the entity with DbContext. Default = false</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>The searched Entity, if found</returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().ToListAsync(cancellationToken);
        }

        return await Entities.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Returns all entities matching the given predicate as IEnumerable
    /// </summary>
    /// <param name="predicate">The predicate for selecting entities</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns></returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        if (asNoTracking)
        {
            return await Entities.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }

        return await Entities.Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds the entity to the database
    /// </summary>
    /// <param name="entity">The entity that should be saved in a database</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    public async Task AddAsync(TEntity entity, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        await Entities.AddAsync(entity, cancellationToken);
        await DataBase.SaveChangesAsync(cancellationToken);

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Adds all entities to the database
    /// </summary>
    /// <param name="entities">Entities that should be saved into a database</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    public async Task AddAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var asList = entities.ToList();

        await Entities.AddRangeAsync(asList, cancellationToken);
        await DataBase.SaveChangesAsync(cancellationToken);

        if (asNoTracking)
        {
            RemoveFromTracking(asList);
        }
    }

    /// <summary>
    /// Removes the given entity from the database
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    public async Task RemoveAsync(TEntity entity, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        Entities.Remove(entity);
        await DataBase.SaveChangesAsync(cancellationToken);

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Removes all given entities from the database
    /// </summary>
    /// <param name="entities">The entities to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    public async Task RemoveAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var asList = entities.ToList();

        Entities.RemoveRange(asList);
        await DataBase.SaveChangesAsync(cancellationToken);

        if (asNoTracking)
        {
            RemoveFromTracking(asList);
        }
    }

    /// <summary>
    /// Updates the given entity from a database
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    public async Task UpdateAsync(TEntity entity, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        Entities.Update(entity);
        await DataBase.SaveChangesAsync(cancellationToken);

        if (asNoTracking)
        {
            RemoveFromTracking(entity);
        }
    }

    /// <summary>
    /// Updates all given entities from the database
    /// </summary>
    /// <param name="entities">The entities to update</param>
    /// <param name="asNoTracking">Optional: Do not track the entities with DbContext. Default = false</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    public async Task UpdateAllAsync(IEnumerable<TEntity> entities, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var asList = entities.ToList();

        Entities.UpdateRange(asList);
        await DataBase.SaveChangesAsync(cancellationToken);

        if (asNoTracking)
        {
            RemoveFromTracking(asList.ToList());
        }
    }

    #endregion

    #region Bulk Writing

    /// <summary>
    /// Performs a bulk write operation for the provided entities into the specified table.
    /// </summary>
    /// <param name="entities">Collection of entities to be written in the database table.</param>
    /// <param name="tableName">Name of the target table. If omitted, the table the entity is mapped to be used.</param>
    /// <param name="timeOutInSeconds">Optional: Number of seconds for the operations to complete. If not set, default is used.</param>
    /// <param name="cancellationToken">Propagates notification that the operation should be canceled.</param>
    /// <returns>Returns the number of rows copied.</returns>
    /// <remarks>
    /// The copy runs on the context's own connection and joins a transaction that is already
    /// open on it, so a rollback takes the copied rows with it. Columns come from the EF model,
    /// which means mapped column names are honored and navigation properties stay out.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the entity is not part of the context's model, or the provider is not SQL Server.</exception>
    public async Task<int> BulkWrite(IEnumerable<TEntity> entities, string? tableName = null, int timeOutInSeconds = 0, CancellationToken cancellationToken = default)
    {
        var asList = entities.ToList();
        if (asList.Count == 0)
        {
            return 0;
        }

        var entityType = DataBase.Model.FindEntityType(typeof(TEntity))
                         ?? throw new InvalidOperationException($"[{typeof(TEntity).Name}] is not part of the model of [{DataBase.GetType().Name}].");

        var destination = string.IsNullOrWhiteSpace(tableName)
            ? entityType.GetTableName() ?? typeof(TEntity).Name
            : tableName;

        var columns = GetBulkColumns(entityType);
        var asDataTable = BuildDataTable(asList, columns, destination);

        if (DataBase.Database.GetDbConnection() is not SqlConnection connection)
        {
            throw new InvalidOperationException("BulkWrite needs a SQL Server connection.");
        }

        // Eine offene Transaktion des Contexts muss mitgegeben werden, sonst läuft der
        // Bulk-Insert daneben und ein Rollback holt die Zeilen nicht zurück.
        var transaction = DataBase.Database.CurrentTransaction?.GetDbTransaction() as SqlTransaction;

        var wasClosed = connection.State != ConnectionState.Open;
        if (wasClosed) await connection.OpenAsync(cancellationToken);

        try
        {
            using var bulkWriter = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);

            if (timeOutInSeconds > 0)
            {
                bulkWriter.BulkCopyTimeout = timeOutInSeconds;
            }

            bulkWriter.DestinationTableName = destination;
            foreach (var column in columns)
            {
                bulkWriter.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await bulkWriter.WriteToServerAsync(asDataTable, cancellationToken);

            return bulkWriter.RowsCopied;
        }
        finally
        {
            if (wasClosed) await connection.CloseAsync();
        }
    }

    /// <summary>
    /// The scalar columns of the entity, taken from the EF model rather than from reflection.
    /// Reflection would also pick up navigation properties and would miss any column renamed via
    /// HasColumnName or [Column]; both make SqlBulkCopy fail on a column the table has not got.
    /// </summary>
    /// <remarks>
    /// Columns the store fills itself are left out - a computed column rejects any value, and a
    /// row version is maintained by the server. Everything else has to be supplied, including
    /// shadow properties: leaving those out writes NULL into them without a word of warning.
    /// </remarks>
    private static List<BulkColumn> GetBulkColumns(IEntityType entityType)
    {
        var storeObject = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
        var discriminator = entityType.FindDiscriminatorProperty();
        var result = new List<BulkColumn>();

        foreach (var property in entityType.GetProperties())
        {
            // Der Store erzeugt den Wert selbst - mitgeschickt lässt SqlBulkCopy die Spalte
            // ablehnen ("cannot be modified because it is either a computed column ...").
            if (property.GetComputedColumnSql() is not null) continue;
            if (property.ValueGenerated == ValueGenerated.OnAddOrUpdate) continue;

            var columnName = storeObject.HasValue
                ? property.GetColumnName(storeObject.Value) ?? property.Name
                : property.Name;

            BulkColumnKind kind;
            if (property.PropertyInfo != null) kind = BulkColumnKind.Clr;
            else if (ReferenceEquals(property, discriminator)) kind = BulkColumnKind.Discriminator;
            else kind = BulkColumnKind.Shadow;

            // Eine Shadow-Property, die der Store füllt, gehört nicht in die DataTable.
            if (kind == BulkColumnKind.Shadow && property.ValueGenerated != ValueGenerated.Never) continue;

            result.Add(new BulkColumn(columnName, property.Name, property.PropertyInfo, property.ClrType, kind));
        }

        return result;
    }

    private DataTable BuildDataTable(List<TEntity> entities, List<BulkColumn> columns, string tableName)
    {
        var table = new DataTable(tableName);

        foreach (var column in columns)
        {
            table.Columns.Add(column.ColumnName, Nullable.GetUnderlyingType(column.ClrType) ?? column.ClrType);
        }

        foreach (var entity in entities)
        {
            var row = table.NewRow();
            foreach (var column in columns)
            {
                row[column.ColumnName] = ReadValue(entity, column) ?? DBNull.Value;
            }
            table.Rows.Add(row);
        }

        return table;
    }

    /// <exception cref="NotSupportedException">
    /// Thrown for a shadow property whose entity is not tracked - its value cannot be read, and
    /// writing NULL instead would corrupt the row without any error.
    /// </exception>
    private object? ReadValue(TEntity entity, BulkColumn column)
    {
        switch (column.Kind)
        {
            case BulkColumnKind.Clr:
                return column.Property!.GetValue(entity);

            case BulkColumnKind.Discriminator:
                // Steht im Modell, nicht an der Entität - der Laufzeittyp entscheidet, welcher
                // Wert es ist.
                return DataBase.Model.FindEntityType(entity.GetType())?.GetDiscriminatorValue();

            default:
                var entry = DataBase.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    throw new NotSupportedException(
                        $"[{column.PropertyName}] on [{typeof(TEntity).Name}] is a shadow property and this entity is not tracked by the context, so its value cannot be read. " +
                        "BulkWrite would silently write NULL into that column. Add or Attach the entities to the context before the call, or map the value to a CLR property.");
                }

                return entry.Property(column.PropertyName).CurrentValue;
        }
    }

    /// <summary>
    /// Where the value of a bulk column comes from.
    /// </summary>
    private enum BulkColumnKind
    {
        /// <summary>A CLR property on the entity.</summary>
        Clr,

        /// <summary>A shadow property - only the change tracker knows the value.</summary>
        Shadow,

        /// <summary>The table-per-hierarchy discriminator - the model knows the value.</summary>
        Discriminator,
    }

    private sealed record BulkColumn(string ColumnName, string PropertyName, PropertyInfo? Property, Type ClrType, BulkColumnKind Kind);

    #endregion


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