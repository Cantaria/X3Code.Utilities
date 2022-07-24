namespace X3Code.Infrastructure.Tests.Data
{
    public class PersonRepository : GenericRepository<Person, UnitTestContext>, IPersonRepository
    {
        public PersonRepository(UnitTestContext context) : base(context)
        {
        }
    }
}