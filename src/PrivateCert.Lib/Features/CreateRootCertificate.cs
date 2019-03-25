using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
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

            public int ExpirationDateInYears { get; set; }

            public string FirstCRL { get; set; }

            public string FirstP7B { get; set; }

            public string Organization { get; set; }

            public string OrganizationUnit { get; set; }

            public string SecondCRL { get; set; }

            public string SecondP7B { get; set; }

            public string SubjectName { get; set; }

            public string ThirdCRL { get; set; }
        }

        public class Command
        {
            public Command(ViewModel viewModel, string masterKeyDecrypted)
            {
                MasterKeyDecrypted = masterKeyDecrypted;
                Country = viewModel.Country;
                Organization = viewModel.Organization;
                OrganizationUnit = viewModel.OrganizationUnit;
                ExpirationDateInYears = viewModel.ExpirationDateInYears;
                SubjectName = viewModel.SubjectName;
                CRLs = new List<string>();
                P7Bs = new List<string>();
                AddToList(viewModel.FirstCRL, CRLs);
                AddToList(viewModel.SecondCRL, CRLs);
                AddToList(viewModel.ThirdCRL, CRLs);
                AddToList(viewModel.FirstP7B, P7Bs);
                AddToList(viewModel.SecondP7B, P7Bs);
            }

            public string Country { get; }

            public ICollection<string> CRLs { get; }

            public int ExpirationDateInYears { get; }

            public string MasterKeyDecrypted { get; }

            public string Organization { get; }

            public string OrganizationUnit { get; }

            public ICollection<string> P7Bs { get; }

            public string FirstP7B
            {
                get
                {
                    if (P7Bs.Count >= 1)
                    {
                        return P7Bs.First();
                    }

                    return null;
                }
            }

            public string SecondP7B
            {
                get
                {
                    if (P7Bs.Count >= 2)
                    {
                        return P7Bs.Skip(1).First();
                    }

                    return null;
                }
            }

            public string SubjectName { get; }

            private void AddToList(string url, ICollection<string> list)
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

                list.Add(trimmedUrl);
            }
        }

        public class CommandHandler
        {
            private readonly IPrivateCertRepository privateCertRepository;

            private readonly CommandValidator validator;

            private readonly IUnitOfWork unitOfWork;

            public CommandHandler(
                CommandValidator validator, IPrivateCertRepository privateCertRepository, IUnitOfWork unitOfWork)
            {
                this.validator = validator;
                this.privateCertRepository = privateCertRepository;
                this.unitOfWork = unitOfWork;
            }

            public ValidationResult Handle(Command command)
            {
                unitOfWork.BeginTransaction();
                var result = validator.Validate(command);
                if (!result.IsValid)
                {
                    return result;
                }

                var passphrase = privateCertRepository.GetPassphrase();
                var passphraseDecrypted = StringCipher.Decrypt(passphrase, command.MasterKeyDecrypted);
                var certificate = Certificate.CreateRootCertificate(command, passphraseDecrypted);
                privateCertRepository.AddCertificate(certificate);
                unitOfWork.SaveChanges();
                unitOfWork.CommitTransaction();

                return result;
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .Must(c => c.CRLs != null && c.CRLs.Count > 0)
                    .WithMessage("Must have at least one CRL URL.")
                    .Custom(CRLMustBeURL)
                    .Custom(P7BMustBeURL);
            }

            private void P7BMustBeURL(Command command, CustomContext customContext)
            {
                MustBeURL(command.CRLs, "CRL", customContext);
            }

            private void MustBeURL(ICollection<string> urls, string list, CustomContext customContext)
            {
                foreach (var url in urls)
                {
                    var valid = Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                                (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (!valid)
                    {
                        customContext.AddFailure($"The {list} {url} is not valid.");
                    }
                }
            }

            private void CRLMustBeURL(Command command, CustomContext customContext)
            {
                MustBeURL(command.P7Bs, "P7B", customContext);
            }
        }
    }
}