using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using AutoMapper;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Sqlite.Infrastructure;
using Serilog;
using Serilog.Events;
using Certificate = PrivateCert.Lib.Model.Certificate;
using Log = PrivateCert.Lib.Model.Log;

namespace PrivateCert.Sqlite.Repositories
{
    public class PrivateCertRepository:IPrivateCertRepository
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
            var logger = new LoggerConfiguration().WriteTo.RollingFile("logs\\log.txt",LogEventLevel.Error).CreateLogger();
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

        public void AddRootCertificate(Certificate certificate)
        {
            var efCertificate = Mapper.Map<Model.Certificate>(certificate);
            context.Certificates.Add(efCertificate);
        }

        public Certificate GetCertificate(int certificateId)
        {
            var efCertificate = context.Certificates.Find(certificateId);
            return Mapper.Map<Certificate>(efCertificate);
        }

        public ICollection<Certificate> GetAllCertificates()
        {
            var efCertificates = context.Certificates.OrderBy(c => c.CertificateTypeId).ThenBy(c => c.Name).ToList();
            return Mapper.Map<ICollection<Certificate>>(efCertificates);
        }
    }
}
