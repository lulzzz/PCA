using System;
using System.Collections.Generic;
using System.Linq;
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

        public string GetMasterKey()
        {
            return context.Settings.Find("MasterKey")?.Value;
        }

        public void InsertError(Log log)
        {
            var logger = new LoggerConfiguration().WriteTo.RollingFile("logs\\log.txt", LogEventLevel.Error).CreateLogger();
            logger.Error("{B}", log.Message);
        }

        public void SetMasterKey(string password)
        {
            var masterkey = context.Settings.Find("MasterKey");
            masterkey.Value = password;
        }

        public string GetPassphrase()
        {
            return context.Settings.Find("Passphrase")?.Value;
        }

        public void SetPassphrase(string passphrase)
        {
            context.Settings.Find("Passphrase").Value = passphrase;
        }

        public void AddCertificate(Certificate certificate)
        {
            if (context.Database.CurrentTransaction == null)
            {
                throw new InvalidOperationException("Needs an open transaction.");
            }

            var efCertificate = Mapper.Map<Model.Certificate>(certificate);
            var maxId = context.Database.SqlQuery<int?>("SELECT MAX(CertificateId) FROM Certificates").SingleOrDefault();
            efCertificate.CertificateId = (maxId ?? 0) + 1;
            if (efCertificate.AuthorityData != null)
            {
                efCertificate.AuthorityData.CertificateId = efCertificate.CertificateId;
            }

            context.Certificates.Add(efCertificate);
        }

        public Certificate GetCertificate(int certificateId)
        {
            var efCertificate = context.Certificates.Find(certificateId);
            return Mapper.Map<Certificate>(efCertificate);
        }

        public ICollection<Certificate> GetValidAuthorityCertificates()
        {
            var efCertificates = context.Certificates
                .Where(
                    c => c.AuthorityId == null && c.ExpirationDate > DateTime.Now && c.IssueDate < DateTime.Now &&
                         (!c.RevocationDate.HasValue || c.RevocationDate.HasValue && c.RevocationDate < DateTime.Now))
                .OrderBy(c => c.Name)
                .ToList();
            return Mapper.Map<ICollection<Certificate>>(efCertificates);
        }

        public ICollection<Certificate> GetAllCertificates()
        {
            var efCertificates = context.Certificates.OrderBy(c => c.CertificateTypeId).ThenBy(c => c.Name).ToList();
            return Mapper.Map<ICollection<Certificate>>(efCertificates);
        }
    }
}