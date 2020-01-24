using System;

namespace X3Code.Infrastructure
{
    public class UnitOfWorkLifeCycle : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkLifeCycle(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            _unitOfWork.Complete();
        }
    }
}
