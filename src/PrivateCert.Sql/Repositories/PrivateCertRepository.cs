using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Mappers;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Sql.Model;
using Log = PrivateCert.Lib.Model.Log;

namespace PrivateCert.Sql.Repositories
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

        public decimal InsertError(Log log)
        {
            // Needs to be separated from any transactional context.
            using (var logContext = new PrivateCertContext())
            {
                var efLog = Mapper.Map<Sql.Model.Log>(log);
                logContext.Logs.Add(efLog);
                return efLog.LogId;
            }
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
    }
}
