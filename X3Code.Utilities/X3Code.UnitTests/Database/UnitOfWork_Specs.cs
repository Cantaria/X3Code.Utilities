using Microsoft.EntityFrameworkCore;
using X3Code.Infrastructure;
using Xunit;

namespace X3Code.UnitTests.Database
{
    public class UnitOfWork_Specs
    {
        [Fact]
        public void GenerateDbcontext()
        {
            var options = new DbContextOptionsBuilder().UseSqlServer("Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;");
            
            var context = new MyContext(options);
            Assert.NotNull(context);

            var unitOfWork = new EfUnitOfWorkBase<MyContext>(options);
            Assert.NotNull(unitOfWork);
        }
    }

    public class MyContext : DbContext
    {
        public MyContext(DbContextOptionsBuilder optionsBuilder) : base(optionsBuilder.Options)
        {
        }

        public MyContext()
        {
            
        }
    }
}
