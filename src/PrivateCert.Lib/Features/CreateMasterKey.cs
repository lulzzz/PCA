using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Features
{
    public class CreateMasterKey
    {
        public class Command
        {
            public Command(string password, string retypePassword)
            {
                Password = password;
                RetypePassword = retypePassword;
            }

            public string Password { get; set; }

            public string RetypePassword { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(BaseValidator baseValidator)
            {
                RuleFor(c => c.Password).NotEmpty().WithMessage("'Password' field is required.");
                RuleFor(c => c.RetypePassword)
                    .NotEmpty()
                    .WithMessage("'Retype Password' field is required.")
                    .Equal(c => c.Password)
                    .WithMessage("Password fields mismatch.");
                RuleFor(c => c).Custom(baseValidator.MasterKeyDoesNotExists);
            }
        }

        public class CommandHandler
        {
            private readonly CommandValidator commandValidator;

            private readonly IPrivateCertRepository privateCertRepository;

            private readonly IUnitOfWork unitOfWork;

            public CommandHandler(
                IPrivateCertRepository privateCertRepository, CommandValidator commandValidator, IUnitOfWork unitOfWork)
            {
                this.privateCertRepository = privateCertRepository;
                this.commandValidator = commandValidator;
                this.unitOfWork = unitOfWork;
            }

            public ValidationResult Handle(Command command)
            {
                unitOfWork.BeginTransaction();
                var result = commandValidator.Validate(command);
                if (!result.IsValid)
                {
                    return result;
                }

                var hash = StringHash.GetHash(command.Password);
                var hashToString = StringHash.GetHashString(hash);
                privateCertRepository.SetMasterKey(hashToString);

                var newPassphrase = Guid.NewGuid().ToString();
                var passphraseEncrypted = StringCipher.Encrypt(newPassphrase, command.Password);
                privateCertRepository.SetPassphrase(passphraseEncrypted);

                unitOfWork.SaveChanges();
                unitOfWork.CommitTransaction();
                return result;
            }
        }

        public class QueryHandler
        {
            private readonly QueryValidator queryValidator;

            public QueryHandler(QueryValidator queryValidator)
            {
                this.queryValidator = queryValidator;
            }

            public ValidationResult Handle(Query query)
            {
                return queryValidator.Validate(query);
            }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(BaseValidator baseValidator)
            {
                RuleFor(c => c).Custom(baseValidator.MasterKeyDoesNotExists);
            }
        }

        public class Query
        {
        }

    }
}
