using Microsoft.EntityFrameworkCore;
using X3Code.Repository.Test.Entities;

namespace X3Code.Repository.Test.Repositories;

public class PersonRepository : BaseRepository<Person>, IPersonRepository
{
    public PersonRepository(DbContext context) : base(context)
    {
    }
}