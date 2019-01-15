using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Sql.Model;

namespace PrivateCert.Sql
{
    public interface IPrivateCertContext : IUnitOfWork
    {
        DbSet<Certificate> Certificates { get; set; }

        DbSet<CertificateType> CertificateTypes { get; set; }

        DbSet<Setting> Settings { get; set; }

        DbSet<Log> Logs { get; set; }
    }
}
