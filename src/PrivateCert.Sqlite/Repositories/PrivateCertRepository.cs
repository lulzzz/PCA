using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Lib.Model;
using Serilog;
using Serilog.Events;
using Log = PrivateCert.Lib.Model.Log;

namespace PrivateCert.Sqlite.Repositories
{
    public class PrivateCertRepository : IPrivateCertRepository
    {
        private readonly IPrivateCertContext context;

        public PrivateCertRepository(IPrivateCertContext context)
        {
            this.context = context;
        }

        public async Task<string> GetMasterKeyAsync()
        {
            var setting = await context.Settings.FindAsync("MasterKey");
            return setting?.Value;
        }

        public void InsertError(Log log)
        {
            var logger = new LoggerConfiguration().WriteTo.RollingFile("logs\\log.txt", LogEventLevel.Error).CreateLogger();
            logger.Error("{B}", log.Message);
        }

        public async Task SetMasterKeyAsync(string password)
        {
            var masterkey = await context.Settings.FindAsync("MasterKey");
            if (masterkey != null)
            {
                masterkey.Value = password;
            }
        }

        public async Task<string> GetPassphraseAsync()
        {
            var setting = await context.Settings.FindAsync("Passphrase");
            return setting?.Value;
        }

        public async Task SetPassphraseAsync(string passphrase)
        {
            var setting = await context.Settings.FindAsync("Passphrase");
            if (setting != null)
            {
                setting.Value = passphrase;
            }
        }

        public async Task AddCertificateAsync(Certificate certificate)
        {
            if (context.Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException("Needs an open transaction.");
            }

            var efCertificate = Mapper.Map<Model.Certificate>(certificate);
            var maxId = await context.Database.SqlQuery<int?>("SELECT MAX(CertificateId) FROM Certificates").SingleOrDefaultAsync();
            efCertificate.CertificateId = (maxId ?? 0) + 1;
            if (efCertificate.AuthorityData != null)
            {
                efCertificate.AuthorityData.CertificateId = efCertificate.CertificateId;
            }

            context.Certificates.Add(efCertificate);
        }

        public async Task<Certificate> GetCertificateAsync(int certificateId)
        {
            var efCertificate = await context.Certificates.FindAsync(certificateId);
            return Mapper.Map<Certificate>(efCertificate);
        }

        public async Task<ICollection<Certificate>> GetValidAuthorityCertificatesAsync()
        {
            var efCertificates = await context.Certificates
                .Where(
                    c => c.AuthorityId == null && c.ExpirationDate > DateTime.Now && c.IssueDate < DateTime.Now &&
                         (!c.RevocationDate.HasValue || c.RevocationDate.HasValue && c.RevocationDate < DateTime.Now))
                .OrderBy(c => c.Name)
                .ToListAsync();
            return Mapper.Map<ICollection<Certificate>>(efCertificates);
        }

        public async Task<ICollection<Certificate>> GetAllCertificatesAsync()
        {
            var efCertificates = await context.Certificates.OrderBy(c => c.CertificateTypeId).ThenBy(c => c.Name).ToListAsync();
            return Mapper.Map<ICollection<Certificate>>(efCertificates);
        }
    }
}