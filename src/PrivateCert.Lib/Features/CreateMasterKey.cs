using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Features
{
    public class CreateMasterKey
    {
        public class Command : IRequest<ValidationResult>
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
                RuleFor(c => c).CustomAsync(baseValidator.MasterKeyDoesNotExists);
            }
        }

        public class CommandHandler : IRequestHandler<Command, ValidationResult>
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

            public async Task<ValidationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                unitOfWork.BeginTransaction();
                var result = await commandValidator.ValidateAsync(command);
                if (!result.IsValid)
                {
                    return result;
                }

                var hash = StringHash.GetHash(command.Password);
                var hashToString = StringHash.GetHashString(hash);
                await privateCertRepository.SetMasterKeyAsync(hashToString);

                var newPassphrase = Guid.NewGuid().ToString();
                var passphraseEncrypted = StringCipher.Encrypt(newPassphrase, command.Password);
                await privateCertRepository.SetPassphraseAsync(passphraseEncrypted);

                unitOfWork.SaveChanges();
                unitOfWork.CommitTransaction();
                return result;
            }
        }

        public class QueryHandler : IRequestHandler<Query,ValidationResult>
        {
            private readonly QueryValidator queryValidator;

            public QueryHandler(QueryValidator queryValidator)
            {
                this.queryValidator = queryValidator;
            }

            public Task<ValidationResult> Handle(Query query, CancellationToken cancellationToken)
            {
                return Task.FromResult(queryValidator.Validate(query));
            }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(BaseValidator baseValidator)
            {
                RuleFor(c => c).CustomAsync(baseValidator.MasterKeyDoesNotExists);
            }
        }

        public class Query : IRequest<ValidationResult>
        {
        }
    }
}
