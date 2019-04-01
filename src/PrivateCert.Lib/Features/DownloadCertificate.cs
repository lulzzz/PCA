using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Lib.Model;

namespace PrivateCert.Lib.Features
{
    public class DownloadCertificate
    {
        public class Query
        {
            public Query(int certificateId, string masterKeyDecrypted)
            {
                CertificateId = certificateId;
                MasterKeyDecrypted = masterKeyDecrypted;
            }

            public int CertificateId { get; set; }

            public string MasterKeyDecrypted { get; }
        }

        public class ViewModel : IValidationResult
        {
            public ValidationResult ValidationResult { get; set; }

            public byte[] CertificateData { get; set; }

            public string FileNameSuggestion { get; set; }

            public string Extension { get; set; }

            public string ExtensionFilter { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly IPrivateCertRepository privateCertRepository;

            public QueryValidator(IPrivateCertRepository privateCertRepository)
            {
                this.privateCertRepository = privateCertRepository;

                RuleFor(c => c)
                    .Must(c=>!string.IsNullOrEmpty(c.MasterKeyDecrypted))
                    .WithMessage("Master Key is empty.")
                    .Custom(CertificateExists);
            }

            private void CertificateExists(Query query, CustomContext customContext)
            {
                if (privateCertRepository.GetCertificate(query.CertificateId) == null)
                {
                    customContext.AddFailure($"Certificate with Id {query.CertificateId} does not exists.");
                }
            }
        }

        public class QueryHandler
        {
            private readonly QueryValidator queryValidator;

            private readonly IPrivateCertRepository privateCertRepository;

            public QueryHandler(QueryValidator queryValidator, IPrivateCertRepository privateCertRepository)
            {
                this.queryValidator = queryValidator;
                this.privateCertRepository = privateCertRepository;
            }

            public ViewModel Handle(Query query)
            {
                var viewModel = new ViewModel();
                viewModel.ValidationResult = queryValidator.Validate(query);
                if (!viewModel.ValidationResult.IsValid)
                {
                    return viewModel;
                }

                var certificate = privateCertRepository.GetCertificate(query.CertificateId);
                var passphrase = privateCertRepository.GetPassphrase();
                var passphraseDecrypted = StringCipher.Decrypt(passphrase, query.MasterKeyDecrypted);

                var x509 = new X509Certificate2();
                x509.Import(certificate.PfxData, passphraseDecrypted, X509KeyStorageFlags.Exportable);
                viewModel.CertificateData = x509.Export(certificate.CertificateType == CertificateTypeEnum.Root ? X509ContentType.Cert : X509ContentType.Pfx, string.Empty);
                viewModel.Extension = (certificate.CertificateType == CertificateTypeEnum.Root ? ".cer" : ".pfx");
                viewModel.ExtensionFilter = (certificate.CertificateType == CertificateTypeEnum.Root ? "Security Certificate (.cer)|*.cer" : "PKCS #12 Certificate (.pfx)|*.pfx");
                viewModel.FileNameSuggestion = certificate.Name.Replace("*","_") + viewModel.Extension;

                return viewModel;
            }
        }
    }
}
