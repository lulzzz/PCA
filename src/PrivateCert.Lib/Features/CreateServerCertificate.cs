using FluentValidation;
using FluentValidation.Results;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Features
{
    public class CreateServerCertificate
    {
        public class Query
        {
        }

        public class ViewModel : IValidationResult
        {
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