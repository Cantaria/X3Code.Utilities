using System;

namespace X3Code.Infrastructure.RavenDb
{
    public class UnitOfWorkLifeCycle : IDisposable
    {
        private readonly IRavenUnitOfWork _unitOfWork;

        public UnitOfWorkLifeCycle(IRavenUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            _unitOfWork.Complete();
        }
    }
}