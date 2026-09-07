using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using X3Code.Repository.Test.Context;

namespace X3Code.Repository.Test.Configuration;

/// <summary>
/// One in-memory SQLite database for the length of a test. Every context it hands out sits on the same open
/// connection, which is what makes them the same database - and what makes a transaction one of them opens
/// mean something to the others.
/// </summary>
internal sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        using var context = NewContext();
        context.Database.EnsureCreated();
    }

    public TransactionalContext NewContext()
    {
        return new TransactionalContext(_connection);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
