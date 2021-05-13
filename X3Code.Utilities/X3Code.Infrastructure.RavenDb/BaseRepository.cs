namespace X3Code.Infrastructure.RavenDb
{
    public class BaseRepository<T> where T : Entity
    {
        private readonly UnitOfWork _unitOfWork;

        public BaseRepository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}