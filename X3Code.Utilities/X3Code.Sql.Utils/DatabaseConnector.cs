using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace X3Code.Sql.Utils;

/// <summary>
/// Connects to a SQL Server database and runs the query.
/// </summary>
/// <remarks>
/// Every method takes an optional set of <see cref="SqlParameter"/>. Use them instead of
/// building the SQL string yourself - a query assembled from user input is a SQL injection.
/// </remarks>
public static class DatabaseConnector
{
    /// <summary>
    /// Fills the DataTable with the result from the given query.
    /// </summary>
    /// <param name="connectionString">The full connection string for the target database.</param>
    /// <param name="query">The full SELECT statement.</param>
    /// <param name="datatable">An empty DataTable that will be filled with data from the database.</param>
    /// <param name="parameters">Parameters referenced by the query, for example <c>new SqlParameter("@name", name)</c>.</param>
    /// <returns>The filled DataTable.</returns>
    /// <exception cref="ArgumentException">Thrown if the connection string or the query is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="datatable"/> is null.</exception>
    public static DataTable SelectAsDataTable(string connectionString, string query, DataTable datatable, params SqlParameter[] parameters)
    {
        ValidateArguments(connectionString, query);
        ArgumentNullException.ThrowIfNull(datatable);

        using var connection = new SqlConnection(connectionString);
        using var command = CreateCommand(connection, query, parameters);
        using var adapter = new SqlDataAdapter(command);

        adapter.Fill(datatable);

        return datatable;
    }

    /// <summary>
    /// Fills the DataTable with the result from the given query.
    /// </summary>
    /// <param name="connectionString">The full connection string for the target database.</param>
    /// <param name="query">The full SELECT statement.</param>
    /// <param name="datatable">An empty DataTable that will be filled with data from the database.</param>
    /// <param name="parameters">Parameters referenced by the query.</param>
    /// <param name="cancellationToken">Cancels the query.</param>
    /// <returns>The filled DataTable.</returns>
    /// <exception cref="ArgumentException">Thrown if the connection string or the query is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="datatable"/> is null.</exception>
    public static async Task<DataTable> SelectAsDataTableAsync(string connectionString, string query, DataTable datatable, SqlParameter[]? parameters = null, CancellationToken cancellationToken = default)
    {
        ValidateArguments(connectionString, query);
        ArgumentNullException.ThrowIfNull(datatable);

        await using var connection = new SqlConnection(connectionString);
        await using var command = CreateCommand(connection, query, parameters);

        await connection.OpenAsync(cancellationToken);

        // SqlDataAdapter has no asynchronous Fill, so the reader does the work here.
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        datatable.Load(reader);

        return datatable;
    }

    /// <summary>
    /// Fires any SQL command to a database. Use with care!
    /// </summary>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="query">The SQL command to be executed.</param>
    /// <param name="parameters">Parameters referenced by the command.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    /// <exception cref="ArgumentException">Thrown if the connection string or the query is null or whitespace.</exception>
    public static int ExecuteSqlCommandOn(string connectionString, string query, params SqlParameter[] parameters)
    {
        ValidateArguments(connectionString, query);

        using var connection = new SqlConnection(connectionString);
        using var command = CreateCommand(connection, query, parameters);

        connection.Open();

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Fires any SQL command to a database. Use with care!
    /// </summary>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="query">The SQL command to be executed.</param>
    /// <param name="parameters">Parameters referenced by the command.</param>
    /// <param name="cancellationToken">Cancels the command.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    /// <exception cref="ArgumentException">Thrown if the connection string or the query is null or whitespace.</exception>
    public static async Task<int> ExecuteSqlCommandOnAsync(string connectionString, string query, SqlParameter[]? parameters = null, CancellationToken cancellationToken = default)
    {
        ValidateArguments(connectionString, query);

        await using var connection = new SqlConnection(connectionString);
        await using var command = CreateCommand(connection, query, parameters);

        await connection.OpenAsync(cancellationToken);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Fires any SQL command inside an already running transaction. Use with care!
    /// </summary>
    /// <param name="transaction">The transaction the command takes part in. Its own connection is used.</param>
    /// <param name="query">The SQL command/query to be executed.</param>
    /// <param name="parameters">Parameters referenced by the command.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="transaction"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the query is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the transaction has no connection left, which means it was already committed, rolled back or disposed.</exception>
    public static int ExecuteSqlCommandOn(SqlTransaction transaction, string query, params SqlParameter[] parameters)
    {
        var connection = GetTransactionConnection(transaction, query);

        using var command = CreateCommand(connection, query, parameters);
        command.Transaction = transaction;

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Fires any SQL command inside an already running transaction. Use with care!
    /// </summary>
    /// <param name="transaction">The transaction the command takes part in. Its own connection is used.</param>
    /// <param name="query">The SQL command/query to be executed.</param>
    /// <param name="parameters">Parameters referenced by the command.</param>
    /// <param name="cancellationToken">Cancels the command.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="transaction"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the query is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the transaction has no connection left, which means it was already committed, rolled back or disposed.</exception>
    public static async Task<int> ExecuteSqlCommandOnAsync(SqlTransaction transaction, string query, SqlParameter[]? parameters = null, CancellationToken cancellationToken = default)
    {
        var connection = GetTransactionConnection(transaction, query);

        await using var command = CreateCommand(connection, query, parameters);
        command.Transaction = transaction;

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    #region Helper

    private static void ValidateArguments(string connectionString, string query)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("A connection string is required.", nameof(connectionString));
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException("A query is required.", nameof(query));
    }

    /// <summary>
    /// A command inside a transaction has to run on the transaction's own connection.
    /// Opening a second one would make SqlClient reject the command.
    /// </summary>
    private static SqlConnection GetTransactionConnection(SqlTransaction transaction, string query)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException("A query is required.", nameof(query));

        return transaction.Connection
               ?? throw new InvalidOperationException("The transaction has no connection. It was already committed, rolled back or disposed.");
    }

    private static SqlCommand CreateCommand(SqlConnection connection, string query, SqlParameter[]? parameters)
    {
        var command = new SqlCommand(query, connection);

        if (parameters is { Length: > 0 })
        {
            command.Parameters.AddRange(parameters);
        }

        return command;
    }

    #endregion
}
