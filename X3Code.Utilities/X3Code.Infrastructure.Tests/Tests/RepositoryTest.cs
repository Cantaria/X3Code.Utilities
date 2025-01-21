using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using X3Code.Infrastructure.Tests.Data;
using X3Code.Infrastructure.Tests.Mockups;
using Xunit;

namespace X3Code.Infrastructure.Tests.Tests;

public class RepositoryTest : RepositoryBaseTest
{
    [Fact]
    public void AddToDatabase()
    {
        var reference = new Person{Name = "First", Surname = "Second", Birthday = DateTime.Today};
        var repo = Services.GetRequiredService<IPersonRepository>();
            
        repo.Add(reference);

        var result = DataBaseConnector.GetFromDb($"{PersonBaseQuery} WHERE Name = 'First';");
        Assert.Equal(1, result.Rows.Count);

        var dbPerson = result.Rows[0].ToPerson(); 
        Assert.Equal(reference.Name, dbPerson.Name);
        Assert.Equal(reference.Surname, dbPerson.Surname);
        Assert.Equal(reference.Birthday, dbPerson.Birthday);
        Assert.Equal(reference.EntityId, dbPerson.EntityId);
    }
        
    [Fact]
    public void MassAddToDatabase()
    {
        var reference = PersonMockupFactory.CreateNPersons(10000);
        var repo = Services.GetRequiredService<IPersonRepository>();
            
        repo.AddAll(reference);

        var dataTable = DataBaseConnector.GetFromDb($"{PersonBaseQuery}");
        Assert.Equal(10000, dataTable.Rows.Count);

        var personsFromDb = dataTable.ToPerson();

        foreach (var referencePerson in reference)
        {
            var personFromDb = personsFromDb.SingleOrDefault(x => x.EntityId == referencePerson.EntityId);
                
            Assert.NotNull(personFromDb);
            Assert.Equal(referencePerson.Name, personFromDb.Name);
            Assert.Equal(referencePerson.Surname, personFromDb.Surname);
            Assert.Equal(referencePerson.Birthday, personFromDb.Birthday);
            Assert.Equal(referencePerson.EntityId, personFromDb.EntityId);
        }
    }

    [Fact]
    public async Task SearchingForNonExistent()
    {
        var dontFindMe = new Person
        {
            Name = "find",
            Birthday = new DateTime(1990, 1, 1),
            Surname = "never",
            EntityId = Guid.NewGuid()
        };

        var repo = Services.GetRequiredService<IPersonRepository>();
        await repo.AddAsync(dontFindMe);

        var nothingFoundResult = await repo.GetAsync(x => x.Name == "wrong");
        Assert.Null(nothingFoundResult);

        var foundHim = await repo.GetAsync(x => x.Name == dontFindMe.Name);
        Assert.NotNull(foundHim);
    }
        
    [Fact]
    public void QueryWithRepository()
    {
        var reference = PersonMockupFactory.CreateNPersons(100);
        var repo = Services.GetRequiredService<IPersonRepository>();
            
        repo.AddAll(reference);

        var fromDatabase = repo.Query(x => !string.IsNullOrWhiteSpace(x.Name) && x.Name.Contains("25")).ToList();
        Assert.Single(fromDatabase);

        var dbPerson = fromDatabase.First();
        //Quick and dirty.... but it's working.
        Assert.Equal(reference[25].Name, dbPerson.Name);
        Assert.Equal(reference[25].Surname, dbPerson.Surname);
        Assert.Equal(reference[25].Birthday, dbPerson.Birthday);
        Assert.Equal(reference[25].EntityId, dbPerson.EntityId);
    }
}