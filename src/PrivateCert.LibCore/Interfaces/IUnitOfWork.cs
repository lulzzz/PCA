using System;

namespace PrivateCert.LibCore.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        int SaveChanges();

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}
