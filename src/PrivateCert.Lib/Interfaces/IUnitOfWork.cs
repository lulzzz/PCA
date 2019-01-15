using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivateCert.Lib.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        int SaveChanges();

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();

        DbSet<T> Set<T>() where T : class;
    }
}
