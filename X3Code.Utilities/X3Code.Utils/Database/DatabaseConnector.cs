using System.Data;
using System.Data.SqlClient;

namespace X3Code.Utils.Database
{
    /// <summary>
    /// Connects to a database and runs the query
    /// </summary>
    public static class DatabaseConnector
    {
        /// <summary>
        /// Fills the DataTable with the result from the given query
        /// </summary>
        /// <param name="datatable">An empty DataTable which will be filled with data from the database</param>
        /// <param name="query">The full SELECT Statement</param>
        /// <param name="connectionString">The full connectionstring for the target database</param>
        /// <returns></returns>
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
        /// Fires any SQL command to an database. Use with care! This methods checks nothing!
        /// </summary>
        /// <param name="connectionString">T</param>
        /// <param name="query"></param>
        /// <returns>Returns the number of rows affected</returns>
        public static int ExecuteSqlCommandOn(string connectionString, string query)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Fires any SQL command to an database within the given SQL-transaction. Use with care! This methods checks nothing!
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="connectionString"></param>
        /// <param name="query"></param>
        /// <returns>Returns the number of rows affected</returns>
        public static int ExecuteSqlCommandOn(ref SqlTransaction transaction, string connectionString, string query)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection, transaction))
            {
                return command.ExecuteNonQuery();
            }
        }
    }
}
