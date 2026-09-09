using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using X3Code.Repository.Test.Configuration;
using X3Code.Repository.Test.Context;
using X3Code.Repository.Test.Entities;
using X3Code.Repository.Test.Repositories;
using Xunit;

namespace X3Code.Repository.Test;

/// <summary>
/// The transaction boundaries were completely blind before. The library logs through
/// ILogger, the abstraction - it brings no sink of its own, so the application decides
/// whether anything is written at all and where it goes.
/// </summary>
public class Logging_specs
{
    #region A logger that only records

    private sealed record Entry(LogLevel Level, EventId EventId, string Message);

    private sealed class RecordingLogger : ILogger<UnitOfWork<TransactionalContext>>
    {
        public List<Entry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add(new Entry(logLevel, eventId, formatter(state, exception)));
    }

    private sealed class DisabledLogger : ILogger<UnitOfWork<TransactionalContext>>
    {
        public int Calls { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        // Das ist der Normalfall in einer Anwendung, die Debug nicht eingeschaltet hat.
        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Calls++;
    }

    #endregion

    private static Person NewPerson(string name) => new()
    {
        EntityId = Guid.NewGuid(),
        Name = name,
        Birthday = new DateTime(2000, 1, 1),
    };

    [Fact]
    public async Task A_committed_unit_of_work_reports_opening_and_committing()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var logger = new RecordingLogger();
        var unitOfWork = new UnitOfWork<TransactionalContext>(context, logger);
        var persons = new PersonRepository(context);

        await using (var scope = await unitOfWork.BeginAsync())
        {
            await persons.AddAsync(NewPerson("Logged"));
            await scope.CompleteAsync();
        }

        Assert.Contains(logger.Entries, e => e.Message.Contains("opened a transaction"));
        Assert.Contains(logger.Entries, e => e.Message.Contains("committed"));
        Assert.DoesNotContain(logger.Entries, e => e.Level == LogLevel.Warning);
    }

    [Fact]
    public async Task A_discarded_unit_of_work_says_so()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var logger = new RecordingLogger();
        var unitOfWork = new UnitOfWork<TransactionalContext>(context, logger);
        var persons = new PersonRepository(context);

        await using (await unitOfWork.BeginAsync())
        {
            await persons.AddAsync(NewPerson("Discarded"));
        }

        Assert.Contains(logger.Entries, e => e.Message.Contains("rolled back"));
        Assert.DoesNotContain(logger.Entries, e => e.Message.Contains("committed"));
    }

    [Fact]
    public async Task An_abandoning_inner_scope_is_a_warning()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var logger = new RecordingLogger();
        var unitOfWork = new UnitOfWork<TransactionalContext>(context, logger);

        await using (await unitOfWork.BeginAsync())
        {
            await using (await unitOfWork.BeginAsync())
            {
                // aufgegeben
            }
        }

        var warning = Assert.Single(logger.Entries, e => e.Level == LogLevel.Warning);
        Assert.Contains("no longer be committed", warning.Message);
        Assert.Equal(1004, warning.EventId.Id);
    }

    [Fact]
    public async Task Nothing_is_written_when_the_level_is_switched_off()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var logger = new DisabledLogger();
        var unitOfWork = new UnitOfWork<TransactionalContext>(context, logger);

        await using (var scope = await unitOfWork.BeginAsync())
        {
            await scope.CompleteAsync();
        }

        // Der LoggerMessage-Generator fragt IsEnabled ab, bevor er irgendetwas aufbaut.
        Assert.Equal(0, logger.Calls);
    }

    [Fact]
    public async Task Without_a_logger_everything_works_as_before()
    {
        using var database = new TestDatabase();
        await using var context = database.NewContext();
        var unitOfWork = new UnitOfWork<TransactionalContext>(context);
        var persons = new PersonRepository(context);

        await using (var scope = await unitOfWork.BeginAsync())
        {
            await persons.AddAsync(NewPerson("Silent"));
            await scope.CompleteAsync();
        }

        await using var verification = database.NewContext();
        Assert.Equal(1, await verification.Set<Person>().CountAsync());
    }
}
