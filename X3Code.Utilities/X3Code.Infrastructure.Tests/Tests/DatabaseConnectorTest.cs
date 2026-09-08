using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using X3Code.Sql.Utils;
using Xunit;

namespace X3Code.Infrastructure.Tests.Tests;

/// <summary>
/// Covers <see cref="DatabaseConnector"/> against a real SQL Server. Both ExecuteSqlCommandOn
/// overloads used to throw on every call - the connection was never opened, and the
/// transactional overload attached the transaction to a second, foreign connection. Neither
/// had a test, which is why it went unnoticed.
/// </summary>
[Collection(Helper.SqlServerCollection.Name)]
public class DatabaseConnectorTest : IDisposable
{
    private const string InsertPerson =
        "INSERT INTO Person (EntityId, Name, Surname, Birthday) VALUES (@id, @name, @surname, @birthday)";

    private readonly Helper.SqlServerFixture _database;
    private readonly Helper.DatabaseConnector _verifier;

    public DatabaseConnectorTest(Helper.SqlServerFixture database)
    {
        _database = database;
        _verifier = new Helper.DatabaseConnector(database.ConnectionString);
    }

    private static SqlParameter[] PersonParameters(Guid id, string name) =>
    [
        new("@id", id),
        new("@name", name),
        new("@surname", "Connector"),
        new("@birthday", new DateTime(2015, 5, 21))
    ];

    [Fact]
    public void ExecuteSqlCommandOnOpensTheConnection()
    {
        var affected = DatabaseConnector.ExecuteSqlCommandOn(
            _database.ConnectionString, InsertPerson, PersonParameters(Guid.NewGuid(), "Sync"));

        Assert.Equal(1, affected);
        Assert.Equal(1, CountPersons("Sync"));
    }

    [Fact]
    public async Task ExecuteSqlCommandOnAsyncOpensTheConnection()
    {
        var affected = await DatabaseConnector.ExecuteSqlCommandOnAsync(
            _database.ConnectionString, InsertPerson, PersonParameters(Guid.NewGuid(), "Async"));

        Assert.Equal(1, affected);
        Assert.Equal(1, CountPersons("Async"));
    }

    [Fact]
    public void SelectAsDataTableHonoursParameters()
    {
        DatabaseConnector.ExecuteSqlCommandOn(_database.ConnectionString, InsertPerson, PersonParameters(Guid.NewGuid(), "Wanted"));
        DatabaseConnector.ExecuteSqlCommandOn(_database.ConnectionString, InsertPerson, PersonParameters(Guid.NewGuid(), "Unwanted"));

        var result = DatabaseConnector.SelectAsDataTable(
            _database.ConnectionString,
            "SELECT * FROM Person WHERE Name = @name",
            new DataTable(),
            new SqlParameter("@name", "Wanted"));

        Assert.Equal(1, result.Rows.Count);
        Assert.Equal("Wanted", result.Rows[0]["Name"]);
    }

    [Fact]
    public async Task SelectAsDataTableAsyncHonoursParameters()
    {
        await DatabaseConnector.ExecuteSqlCommandOnAsync(_database.ConnectionString, InsertPerson, PersonParameters(Guid.NewGuid(), "AsyncWanted"));

        var result = await DatabaseConnector.SelectAsDataTableAsync(
            _database.ConnectionString,
            "SELECT * FROM Person WHERE Name = @name",
            new DataTable(),
            [new SqlParameter("@name", "AsyncWanted")]);

        Assert.Equal(1, result.Rows.Count);
        Assert.Equal("AsyncWanted", result.Rows[0]["Name"]);
    }

    [Fact]
    public void ParametersAreNotConcatenatedIntoTheQuery()
    {
        // A value that would end the statement if it were pasted into the SQL string.
        const string hostile = "'; DROP TABLE Person; --";

        DatabaseConnector.ExecuteSqlCommandOn(_database.ConnectionString, InsertPerson, PersonParameters(Guid.NewGuid(), hostile));

        // The table still exists and holds the value verbatim.
        Assert.Equal(1, CountPersons(hostile));
    }

    [Fact]
    public void CommandTakesPartInTheGivenTransaction()
    {
        using var connection = new SqlConnection(_database.ConnectionString);
        connection.Open();

        using (var transaction = connection.BeginTransaction())
        {
            var affected = DatabaseConnector.ExecuteSqlCommandOn(
                transaction, InsertPerson, PersonParameters(Guid.NewGuid(), "RolledBack"));

            Assert.Equal(1, affected);
            transaction.Rollback();
        }

        Assert.Equal(0, CountPersons("RolledBack"));
    }

    [Fact]
    public async Task CommandTakesPartInTheGivenTransactionAsync()
    {
        await using var connection = new SqlConnection(_database.ConnectionString);
        await connection.OpenAsync();

        await using (var transaction = (SqlTransaction)await connection.BeginTransactionAsync())
        {
            await DatabaseConnector.ExecuteSqlCommandOnAsync(
                transaction, InsertPerson, PersonParameters(Guid.NewGuid(), "Committed"));

            await transaction.CommitAsync();
        }

        Assert.Equal(1, CountPersons("Committed"));
    }

    [Fact]
    public void DisposedTransactionIsReportedClearly()
    {
        SqlTransaction transaction;
        using (var connection = new SqlConnection(_database.ConnectionString))
        {
            connection.Open();
            transaction = connection.BeginTransaction();
            transaction.Rollback();
        }

        var exception = Assert.Throws<InvalidOperationException>(
            () => DatabaseConnector.ExecuteSqlCommandOn(transaction, InsertPerson, PersonParameters(Guid.NewGuid(), "Gone")));

        Assert.Contains("no connection", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MissingQueryIsAnArgumentException(string? query)
    {
        Assert.Throws<ArgumentException>(
            () => DatabaseConnector.ExecuteSqlCommandOn(_database.ConnectionString, query!));
    }

    [Fact]
    public void MissingConnectionStringIsAnArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => DatabaseConnector.ExecuteSqlCommandOn(string.Empty, "SELECT 1"));
    }

    private int CountPersons(string name)
    {
        var table = _verifier.GetFromDb($"SELECT COUNT(1) FROM Person WHERE Name = N'{name.Replace("'", "''")}'");

        return (int)table.Rows[0][0];
    }

    public void Dispose()
    {
        _verifier.CleanDataBase();
        GC.SuppressFinalize(this);
    }
}
