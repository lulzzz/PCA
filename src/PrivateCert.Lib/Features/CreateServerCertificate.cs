using FluentValidation;
using FluentValidation.Results;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Lib.Model;

namespace PrivateCert.Lib.Features
{
    public class CreateServerCertificate
    {
        public class Query
        {
        }

        public class ViewModel : IValidationResult
        {
            public int CertificateRootId { get; set; }

            public int ExpirationDateInDays { get; set; }

            public string IssuerName { get; set; }

            public ValidationResult ValidationResult { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
        }

        public class QueryHandler
        {
            private readonly QueryValidator queryValidator;

            public QueryHandler(QueryValidator queryValidator)
            {
                this.queryValidator = queryValidator;
            }

            public ViewModel Handle(Query query)
            {
                var viewModel = new ViewModel();
                viewModel.ValidationResult = queryValidator.Validate(query);
                if (!viewModel.ValidationResult.IsValid)
                {
                    return viewModel;
                }

                return viewModel;
            }
        }

        public class Command
        {
            public Command(ViewModel viewModel, string masterKeyDecrypted)
            {
                MasterKeyDecrypted = masterKeyDecrypted;
                IssuerName = viewModel.IssuerName;
                ExpirationDateInDays = viewModel.ExpirationDateInDays;
            }

            public int RootCertificateId { get; }

            public int ExpirationDateInDays { get; }

            public string IssuerName { get; }

            public string MasterKeyDecrypted { get; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
        }

        public class CommandHandler
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

            public ValidationResult Handle(Command command)
            {
                var result = commandValidator.Validate(command);
                if (!result.IsValid)
                {
                    return result;
                }

                var passphrase = privateCertRepository.GetPassphrase();
                var passphraseDecrypted = StringCipher.Decrypt(passphrase, command.MasterKeyDecrypted);
                var certificate = Certificate.CreateServerCertificate(command, passphraseDecrypted);
                privateCertRepository.AddRootCertificate(certificate);
                unitOfWork.SaveChanges();

                return result;
            }
        }
    }
}