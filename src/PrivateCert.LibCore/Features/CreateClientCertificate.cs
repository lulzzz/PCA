using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PrivateCert.LibCore.Infrastructure;
using PrivateCert.LibCore.Interfaces;
using PrivateCert.LibCore.Model;

namespace PrivateCert.LibCore.Features
{
    public class CreateClientCertificate
    {
        public class Query : IRequest<ViewModel>
        {
        }

        public class ViewModel : IValidationResult
        {
            public int ExpirationDateInDays { get; set; }

            public string SubjectName { get; set; }

            public ICollection<Certificate> AuthorityCertificates { get; set; }

            public int SelectedAuthorityCertificateId { get; set; }

            public ValidationResult ValidationResult { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
        }

        public class QueryHandler:IRequestHandler<Query,ViewModel>
        {
            private readonly IPrivateCertRepository privateCertRepository;
            private readonly QueryValidator queryValidator;

            public QueryHandler(QueryValidator queryValidator, IPrivateCertRepository privateCertRepository)
            {
                this.queryValidator = queryValidator;
                this.privateCertRepository = privateCertRepository;
            }

            public async Task<ViewModel> Handle(Query query, CancellationToken cancellationToken)
            {
                var viewModel = new ViewModel();
                viewModel.ValidationResult = queryValidator.Validate(query);
                if (!viewModel.ValidationResult.IsValid)
                {
                    return viewModel;
                }

                viewModel.AuthorityCertificates = await privateCertRepository.GetValidAuthorityCertificatesAsync();
                viewModel.SelectedAuthorityCertificateId = viewModel.AuthorityCertificates.First().CertificateId;
                viewModel.ExpirationDateInDays = 720;
                viewModel.SubjectName = "Signer or Bob";

                return viewModel;
            }
        }

        public class Command : IRequest<ValidationResult>
        {
            public Command(ViewModel viewModel, string masterKeyDecrypted)
            {
                MasterKeyDecrypted = masterKeyDecrypted;
                SubjectName = viewModel.SubjectName;
                ExpirationDateInDays = viewModel.ExpirationDateInDays;
                SelectedAuthorityCertificateId = viewModel.SelectedAuthorityCertificateId;
            }

            public int SelectedAuthorityCertificateId { get; }

            public int ExpirationDateInDays { get; }

            public string SubjectName { get; }

            public string MasterKeyDecrypted { get; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
        }

        public class CommandHandler : IRequestHandler<Command, ValidationResult>
        {
            private readonly CommandValidator commandValidator;

            private readonly IPrivateCertRepository privateCertRepository;

            private readonly IUnitOfWork unitOfWork;

            public CommandHandler(CommandValidator commandValidator, IUnitOfWork unitOfWork, IPrivateCertRepository privateCertRepository)
            {
                this.commandValidator = commandValidator;
                this.unitOfWork = unitOfWork;
                this.privateCertRepository = privateCertRepository;
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
                var certificate = Certificate.CreateClientCertificate(command, parentCertificate, passphraseDecrypted);
                await privateCertRepository.AddCertificateAsync(certificate);
                unitOfWork.SaveChanges();
                unitOfWork.CommitTransaction();

                return result;
            }
        }
    }
}