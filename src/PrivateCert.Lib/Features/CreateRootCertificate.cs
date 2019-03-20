using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Lib.Model;

namespace PrivateCert.Lib.Features
{
    public class CreateRootCertificate
    {
        public class ViewModel
        {
            public string Country { get; set; }
            public string Organization { get; set; }
            public string OrganizationUnit { get; set; }
            public string SubjectName { get; set; }
            public string FirstCRL { get; set; }
            public string SecondCRL { get; set; }
            public string ThirdCRL { get; set; }
            public string FirstP7B { get; set; }
            public string SecondP7B { get; set; }
            public int ExpirationDateInYears { get; set; }
        }

        public class Command
        {
            public string MasterKeyDecrypted { get; }

            public Command(ViewModel viewModel, string masterKeyDecrypted)
            {

                MasterKeyDecrypted = masterKeyDecrypted;
                Country = viewModel.Country;
                Organization = viewModel.Organization;
                OrganizationUnit = viewModel.OrganizationUnit;
                ExpirationDateInYears = viewModel.ExpirationDateInYears;
                SubjectName = viewModel.SubjectName;
                CRLs = new List<string>();
                AddCRL(viewModel.FirstCRL);
                AddCRL(viewModel.SecondCRL);
                AddCRL(viewModel.ThirdCRL);
                Addl
            }

            private void AddCRL(string url)
            {
                if (url == null)
                {
                    return;
                }

                var trimmedUrl = url.Trim();
                if (trimmedUrl == string.Empty)
                {
                    return;
                }

                CRLs.Add(trimmedUrl);
            }

            public string Country { get; private set; }
            public string Organization { get; private set; }
            public string OrganizationUnit { get; private set; }
            public int ExpirationDateInYears { get; private set; }

            public string SubjectName { get; private set; }

            public ICollection<string> CRLs { get; private set; }
        }

        public class CommandHandler
        {
            private readonly IPrivateCertRepository privateCertRepository;
            private readonly CommandValidator validator;

            private readonly IUnitOfWork unitOfWork;

            public CommandHandler(CommandValidator validator, IPrivateCertRepository privateCertRepository, IUnitOfWork unitOfWork)
            {
                this.validator = validator;
                this.privateCertRepository = privateCertRepository;
                this.unitOfWork = unitOfWork;
            }

            public ValidationResult Handle(Command command)
            {
                var result  = validator.Validate(command);
                if (!result.IsValid)
                {
                    return result;
                }

                var passphrase = privateCertRepository.GetPassphrase();
                var passphraseDecrypted = StringCipher.Decrypt(passphrase, command.MasterKeyDecrypted);
                var certificate = Certificate.CreateRootCertificate(command, passphraseDecrypted);
                privateCertRepository.AddRootCertificate(certificate);
                unitOfWork.SaveChanges();

                return result;
            }

        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .Must(c=>c.CRLs != null && c.CRLs.Count>0)
                    .WithMessage("Must have at least one CRL URL.")
                    .Custom(CRLMustBeURL);
            }

            private void CRLMustBeURL(Command command, CustomContext customContext)
            {
                foreach (var crlLink in command.CRLs)
                {
                    bool valid = Uri.TryCreate(crlLink, UriKind.Absolute, out var uriResult) 
                                 && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (!valid)
                    {
                        customContext.AddFailure($"The CRL {crlLink} is not valid.");
                    }
                }    
            }
        }
    }
}
