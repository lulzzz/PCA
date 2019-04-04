using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Lib.Model;

namespace PrivateCert.Lib.Features
{
    public class CreateServerCertificate
    {
        public class Query : IRequest<ViewModel>
        {
        }

        public class ViewModel : IValidationResult
        {
            public int ExpirationDateInDays { get; set; }

            public string IssuerName { get; set; }

            public ICollection<Certificate> AuthorityCertificates { get; set; }

            public int SelectedAuthorityCertificateId { get; set; }

            public ValidationResult ValidationResult { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
        }

        public class QueryHandler : IRequestHandler<Query, ViewModel>
        {
            private readonly QueryValidator queryValidator;

            private readonly IPrivateCertRepository privateCertRepository;

            public QueryHandler(QueryValidator queryValidator, IPrivateCertRepository privateCertRepository)
            {
                this.queryValidator = queryValidator;
                this.privateCertRepository = privateCertRepository;
            }

            public async Task<ViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                var viewModel = new ViewModel();
                viewModel.ValidationResult = await queryValidator.ValidateAsync(request,cancellationToken);
                if (!viewModel.ValidationResult.IsValid)
                {
                    return viewModel;
                }

                viewModel.AuthorityCertificates = await privateCertRepository.GetValidAuthorityCertificatesAsync();
                viewModel.SelectedAuthorityCertificateId = viewModel.AuthorityCertificates.First().CertificateId;
                viewModel.ExpirationDateInDays = 720;
                viewModel.IssuerName = "some.domain.com or *.domain.com";

                return viewModel;
            }
        }

        public class Command : IRequest<ValidationResult>
        {
            public Command(ViewModel viewModel, string masterKeyDecrypted)
            {
                MasterKeyDecrypted = masterKeyDecrypted;
                IssuerName = viewModel.IssuerName;
                ExpirationDateInDays = viewModel.ExpirationDateInDays;
                SelectedAuthorityCertificateId = viewModel.SelectedAuthorityCertificateId;
            }

            public int SelectedAuthorityCertificateId { get; }

            public int ExpirationDateInDays { get; }

            public string IssuerName { get; }

            public string MasterKeyDecrypted { get; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
        }

        public class CommandHandler : IRequestHandler<Command,ValidationResult>
        {
            private readonly CommandValidator commandValidator;
            private readonly IPrivateCertRepository privateCertRepository;
            private readonly IUnitOfWork unitOfWork;

            public CommandHandler(CommandValidator commandValidator, IPrivateCertRepository privateCertRepository, IUnitOfWork unitOfWork)
            {
                this.commandValidator = commandValidator;
                this.privateCertRepository = privateCertRepository;
                this.unitOfWork = unitOfWork;
            }

            public async Task<ValidationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                unitOfWork.BeginTransaction();
                var result = await commandValidator.ValidateAsync(command, cancellationToken);
                if (!result.IsValid)
                {
                    return result;
                }

                var passphrase = await privateCertRepository.GetPassphraseAsync();
                var passphraseDecrypted = StringCipher.Decrypt(passphrase, command.MasterKeyDecrypted);
                var parentCertificate = await privateCertRepository.GetCertificateAsync(command.SelectedAuthorityCertificateId);
                var certificate = Certificate.CreateServerCertificate(command, parentCertificate, passphraseDecrypted);
                await privateCertRepository.AddCertificateAsync(certificate);
                unitOfWork.SaveChanges();
                unitOfWork.CommitTransaction();

                return result;

            }
        }
    }
}