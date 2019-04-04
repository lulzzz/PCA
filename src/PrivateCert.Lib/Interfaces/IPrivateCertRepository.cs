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
        Task<string> GetMasterKeyAsync();

        void InsertError(Log log);

        Task SetMasterKeyAsync(string password);

        Task<string> GetPassphraseAsync();

        Task SetPassphraseAsync(string passphrase);

        Task AddCertificateAsync(Certificate certificate);

        Task<ICollection<Certificate>> GetAllCertificatesAsync();

        Task<Certificate> GetCertificateAsync(int certificateId);

        Task<ICollection<Certificate>> GetValidAuthorityCertificatesAsync();
    }
}
