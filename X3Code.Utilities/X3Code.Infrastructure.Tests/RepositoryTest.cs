using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using X3Code.Infrastructure.Tests.ConstValues;
using X3Code.Infrastructure.Tests.Data;
using X3Code.Infrastructure.Tests.Helper;
using X3Code.Infrastructure.Tests.Mockups;
using Xunit;

namespace X3Code.Infrastructure.Tests
{
    public class RepositoryTest : IDisposable
    {
        private readonly IServiceProvider _service;
        private readonly DatabaseConnector _database;
        private const string BaseQuery = "SELECT * FROM Person ";

        public RepositoryTest()
        {
            _service = BuildServiceProvider();
            _database = new DatabaseConnector(DbConnectionStrings.GetConnectionString());
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            var connectionString = DbConnectionStrings.GetConnectionString();
            services.AddDbContext<UnitTestContext>(options => { options.UseSqlServer(connectionString); });
            services.AddTransient<IPersonRepository, PersonRepository>();

            return services.BuildServiceProvider();
        }

        [Fact]
        public void AddToDatabase()
        {
            var reference = new Person("First", "Second", DateTime.Today);
            var repo = _service.GetRequiredService<IPersonRepository>();
            
            repo.Add(reference);

            var result = _database.GetFromDb($"{BaseQuery} WHERE Name = 'First';");
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
            var repo = _service.GetRequiredService<IPersonRepository>();
            
            repo.AddAll(reference);

            var dataTable = _database.GetFromDb($"{BaseQuery}");
            Assert.Equal(10000, dataTable.Rows.Count);

            var personsFromDb = dataTable.ToPerson();

            foreach (var referencePerson in reference)
            {
                var personFromDb = personsFromDb.SingleOrDefault(x => x.EntityId == referencePerson.EntityId);
                
                Assert.NotNull(personFromDb);
                if (personFromDb == null) return;
                Assert.Equal(referencePerson.Name, personFromDb.Name);
                Assert.Equal(referencePerson.Surname, personFromDb.Surname);
                Assert.Equal(referencePerson.Birthday, personFromDb.Birthday);
                Assert.Equal(referencePerson.EntityId, personFromDb.EntityId);
            }
        }

        public void Dispose()
        {
            _database.CleanDataBase();
        }
    }
}