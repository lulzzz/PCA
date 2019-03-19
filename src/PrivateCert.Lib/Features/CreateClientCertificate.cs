using FluentValidation;
using FluentValidation.Results;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Features
{
    public class CreateClientCertificate
    {
        public class Query
        {
        }

        public class ViewModel : IValidationResult
        {
            public string Domain { get; set; }

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
                ViewModel = viewModel;
                MasterKeyDecrypted = masterKeyDecrypted;
            }

            public string MasterKeyDecrypted { get; private set; }

            public ViewModel ViewModel { get; private set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
        }

        public class CommandHandler
        {
            private readonly CommandValidator commandValidator;

            public CommandHandler(CommandValidator commandValidator)
            {
                this.commandValidator = commandValidator;
            }

            public ValidationResult Handle(Command command)
            {
                var result = commandValidator.Validate(command);
                if (!result.IsValid)
                {
                    return result;
                }

                return result;
            }
        }
    }
}