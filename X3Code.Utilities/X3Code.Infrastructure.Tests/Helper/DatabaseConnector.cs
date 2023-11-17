using System.Data;
using Microsoft.Data.SqlClient;

namespace X3Code.Infrastructure.Tests.Helper;

public class DatabaseConnector
{
    private readonly string _connectionString;

    public DatabaseConnector(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DataTable GetFromDb(string query)
    {
        var table = new DataTable();
        using (var connection = new SqlConnection(_connectionString))
        {
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                var da = new SqlDataAdapter(command);
                da.Fill(table);
            }
        }

        return table;
    }

    public void CleanDataBase()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            using (var command = new SqlCommand("TRUNCATE TABLE [dbo].[Person]", connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}