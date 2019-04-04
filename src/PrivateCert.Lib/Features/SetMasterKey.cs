using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Features
{
    public class SetMasterKey
    {
        public class Command : IRequest<ValidationResult>
        {
            public Command(string currentPassword, string password, string retypePassword)
            {
                CurrentPassword = currentPassword;
                Password = password;
                RetypePassword = retypePassword;
            }

            public string CurrentPassword { get; set; }

            public string Password { get; set; }

            public string RetypePassword { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(BaseValidator baseValidator)
            {
                RuleFor(c => c.CurrentPassword)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage("'Current Password' field is required.")
                    .MustAsync(baseValidator.MasterKeyDoesExists)
                    .WithMessage("'Master key' does not exists.")
                    .MustAsync(baseValidator.MasterKeySucessfulyDecrypted)
                    .WithMessage("'Current Password' is invalid.");
                RuleFor(c => c.Password).NotEmpty().WithMessage("'Password' field is required.");
                RuleFor(c => c.RetypePassword)
                    .NotEmpty()
                    .WithMessage("'Retype Password' field is required.")
                    .Equal(c => c.Password)
                    .WithMessage("Password fields mismatch.");
            }
        }

        public class CommandHandler : IRequestHandler<Command,ValidationResult>
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

            public async Task<ValidationResult> Handle(Command request, CancellationToken cancellationToken)
            {
                unitOfWork.BeginTransaction();
                var result = await commandValidator.ValidateAsync(request, cancellationToken);
                if (!result.IsValid)
                {
                    return result;
                }

                var hash = StringHash.GetHash(request.Password);
                var hashToString = StringHash.GetHashString(hash);
                await privateCertRepository.SetMasterKeyAsync(hashToString);

                var passphrase = await privateCertRepository.GetPassphraseAsync();
                var passphraseDecrypted = StringCipher.Decrypt(passphrase, request.CurrentPassword);
                var passphraseEncrypted = StringCipher.Encrypt(passphraseDecrypted, request.Password);
                await privateCertRepository.SetPassphraseAsync(passphraseEncrypted);

                unitOfWork.SaveChanges();
                unitOfWork.CommitTransaction();
                return result;
            }
        }

      
    }
}