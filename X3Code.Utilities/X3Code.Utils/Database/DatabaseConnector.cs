using System.Data;
using Microsoft.Data.SqlClient;

namespace X3Code.Utils.Database;

/// <summary>
/// Connects to a database and runs the query
/// </summary>
public static class DatabaseConnector
{
    /// <summary>
    /// Fills the DataTable with the result from the given query.
    /// </summary>
    /// <param name="connectionString">The full connection string for the target database.</param>
    /// <param name="query">The full SELECT statement.</param>
    /// <param name="datatable">An empty DataTable which will be filled with data from the database.</param>
    /// <returns>The filled DataTable.</returns>
    public static DataTable SelectAsDataTable(string connectionString, string query, DataTable datatable)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            var adapter = new SqlDataAdapter
            {
                SelectCommand = new SqlCommand(query, connection)
            };
            adapter.Fill(datatable);
            return datatable;
        }
    }

    /// <summary>
    /// Fires any SQL command to a database. Use with care! This method checks nothing!
    /// </summary>
    /// <param name="connectionString">The connection string for the database</param>
    /// <param name="query">The SQL command to be executed</param>
    /// <returns>The number of rows affected by the SQL command</returns>
    public static int ExecuteSqlCommandOn(string connectionString, string query)
    {
        using (var connection = new SqlConnection(connectionString))
        using (var command = new SqlCommand(query, connection))
        {
            return command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Fires any SQL command to a database within the given SQL-transaction. Use with care! This method checks nothing!
    /// </summary>
    /// <param name="transaction">The SQL-transaction in which the command will be executed.</param>
    /// <param name="connectionString">The connection string to the database.</param>
    /// <param name="query">The SQL command/query to be executed.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    public static int ExecuteSqlCommandOn(ref SqlTransaction transaction, string connectionString, string query)
    {
        using (var connection = new SqlConnection(connectionString))
        using (var command = new SqlCommand(query, connection, transaction))
        {
            return command.ExecuteNonQuery();
        }
    }
}