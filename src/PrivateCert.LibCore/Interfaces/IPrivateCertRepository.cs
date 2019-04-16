using System.Collections.Generic;
using System.Threading.Tasks;
using PrivateCert.LibCore.Model;

namespace PrivateCert.LibCore.Interfaces
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
