using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PrivateCert.AzureDal.Model;
using PrivateCert.LibCore.Interfaces;

namespace PrivateCert.AzureDal
{
    public interface IPrivateCertContext : IUnitOfWork
    {
        DbSet<Certificate> Certificates { get; set; }

        DbSet<CertificateType> CertificateTypes { get; set; }

        DbSet<Setting> Settings { get; set; }

        DbSet<Log> Logs { get; set; }

        Database Database { get; }
    }
}
