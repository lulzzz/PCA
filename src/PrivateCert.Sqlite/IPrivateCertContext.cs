using System.Data.Entity;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Sqlite.Model;

namespace PrivateCert.Sqlite
{
    public interface IPrivateCertContext : IUnitOfWork
    {
        DbSet<Certificate> Certificates { get; set; }

        DbSet<CertificateType> CertificateTypes { get; set; }

        DbSet<Setting> Settings { get; set; }

        Database Database { get; }
    }
}
