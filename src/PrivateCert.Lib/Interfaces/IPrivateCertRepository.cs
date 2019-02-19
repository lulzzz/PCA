using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrivateCert.Lib.Model;

namespace PrivateCert.Lib.Interfaces
{
    public interface IPrivateCertRepository
    {
        string GetMasterKey();

        decimal InsertError(Log log);

        void SetMasterKey(string password);

        string GetPassphrase();

        void SetPassphrase(string passphrase);

        void AddRootCertificate(Certificate certificate);

        ICollection<Certificate> GetAllCertificates();

        Certificate GetCertificate(int certificateId);
    }
}
