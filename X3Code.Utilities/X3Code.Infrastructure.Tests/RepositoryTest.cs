using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using X3Code.Infrastructure.Tests.ConstValues;
using X3Code.Infrastructure.Tests.Data;
using X3Code.Infrastructure.Tests.Helper;
using Xunit;

namespace X3Code.Infrastructure.Tests
{
    public class RepositoryTest
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
            
            _database.CleanDataBase();
        }
    }
}