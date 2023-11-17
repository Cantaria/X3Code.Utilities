

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using X3Code.Infrastructure.Tests.Data;
using X3Code.Infrastructure.Tests.Mockups;
using Xunit;
using Xunit.Abstractions;

namespace X3Code.Infrastructure.Tests.Tests;

public class BulkWriterTest : RepositoryBaseTest
{
    private readonly ITestOutputHelper _output;

    public BulkWriterTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    [InlineData(10000000)]
    public async Task BulkWriteToDatabase(int entityCount)
    {
        var persons = PersonMockupFactory.CreateNPersons(entityCount);
        var repo = Services.GetRequiredService<IPersonRepository>();

        var watch = new Stopwatch();
        watch.Start();
        var rowCount = await repo.BulkWrite(persons, "Person", 120);
        watch.Stop();
       
        _output.WriteLine($"Test with [{entityCount}] entities. Written: [{rowCount}]. Elapsed time: [{watch.Elapsed:G}]");

        var rowCountInDb = DataBaseConnector.GetFromDb("SELECT COUNT(1) FROM Person");
        var counter = (int)rowCountInDb.Rows[0][0];
        DataBaseConnector.CleanDataBase();
        
        Assert.Equal(entityCount, rowCount);
        Assert.Equal(counter, rowCount);
    }
}