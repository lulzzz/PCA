using System;
using System.Data.Entity;
using PrivateCert.Sqlite.Initializer;
using PrivateCert.Sqlite.Model;

namespace PrivateCert.Sqlite
{
    public class PrivateCertContext : DbContext, IPrivateCertContext
    {
        public PrivateCertContext() : base("name=PrivateCertDatabase")
        {
            Database.SetInitializer(new EFInitializer());
        }

        public virtual DbSet<Certificate> Certificates { get; set; }

        public virtual DbSet<CertificateType> CertificateTypes { get; set; }

        public virtual DbSet<Setting> Settings { get; set; }

        public new virtual Database Database => base.Database;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Certificate>().Property(e => e.SerialNumber).IsUnicode(false);

            modelBuilder.Entity<Certificate>().Property(e => e.Name).IsUnicode(false);

            modelBuilder.Entity<Certificate>().Property(e => e.Thumbprint).IsUnicode(false);

            modelBuilder.Entity<CertificateType>().Property(e => e.Description).IsUnicode(false);

            modelBuilder.Entity<Setting>().Property(e => e.Value).IsUnicode(false);
        }

        public void BeginTransaction()
        {
            if (Database.CurrentTransaction != null)
            {
                throw new InvalidOperationException("Transaction already open.");
            }

            Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException("No opened transaction.");
            }

            Database.CurrentTransaction.Commit();
        }

        public void RollbackTransaction()
        {
            if (Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException("No opened transaction.");
            }

            Database.CurrentTransaction.Rollback();
        }
    }
}