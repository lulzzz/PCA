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
using PrivateCert.Lib.Interfaces;

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
            public DateTime ExpirationDate { get; set; }
        }

        public class Command
        {
            public Command(ViewModel viewModel)
            {
                SubjectName = viewModel.SubjectName;
                AddCRL(viewModel.FirstCRL);
                AddCRL(viewModel.SecondCRL);
                AddCRL(viewModel.ThirdCRL);
            }

            private void AddCRL(string url)
            {
                var trimmedUrl = url.Trim();
                if (!string.IsNullOrEmpty(trimmedUrl))
                {
                    CRLs.Add(trimmedUrl);
                }
            }

            public string SubjectName { get; private set; }

            public ICollection<string> CRLs { get; private set; }
        }

        public class CommandHandler
        {
            private readonly CommandValidator validator;

            public CommandHandler(CommandValidator validator)
            {
                this.validator = validator;
            }

            public Result Handle(Command command)
            {
                var result = new Result();
                result.ValidationResult = validator.Validate(command);
                if (!result.ValidationResult.IsValid)
                {
                    return result;
                }

                //var certificate = new Certificate;
                //GenerateRootCertificate()

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

        public class Result : IValidationResult
        {
            public ValidationResult ValidationResult { get; set; }
        }
    }
}
