using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PrivateCert.AzureDal.Model;

namespace PrivateCert.AzureDal
{
    public class PrivateCertContext : DbContext, IPrivateCertContext
    {
        //public PrivateCertContext() : base("name=PrivateCertDatabase")
        //{
        //    Database.SetInitializer(new EFInitializer());
        //}

        //public virtual DbSet<Certificate> Certificates { get; set; }

        //public virtual DbSet<CertificateType> CertificateTypes { get; set; }

        //public virtual DbSet<Setting> Settings { get; set; }

        //public virtual DbSet<Log> Logs { get; set; }

        //public new virtual Database Database => base.Database;

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //}

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Certificate>().Property(e => e.SerialNumber).IsUnicode(false);

        //    modelBuilder.Entity<Certificate>().Property(e => e.Name).IsUnicode(false);

        //    modelBuilder.Entity<Certificate>().Property(e => e.Thumbprint).IsUnicode(false);

        //    modelBuilder.Entity<CertificateType>().Property(e => e.Description).IsUnicode(false);

        //    modelBuilder.Entity<Setting>().Property(e => e.Value).IsUnicode(false);

        //    modelBuilder.Entity<Log>().Property(e => e.Message).IsUnicode(false);
        //}

        //public void BeginTransaction()
        //{
        //    if (Database.CurrentTransaction != null)
        //    {
        //        throw new InvalidOperationException("Transaction already open.");
        //    }

        //    Database.BeginTransaction();
        //}

        //public void CommitTransaction()
        //{
        //    if (Database.CurrentTransaction == null)
        //    {
        //        throw new InvalidOperationException("No opened transaction.");
        //    }

        //    Database.CurrentTransaction.Commit();
        //}

        //public void RollbackTransaction()
        //{
        //    if (Database.CurrentTransaction == null)
        //    {
        //        throw new InvalidOperationException("No opened transaction.");
        //    }

        //    Database.CurrentTransaction.Rollback();
        //}

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<CertificateType> CertificateTypes { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Log> Logs { get; set; }

        public Database Database { get; }
    }
}