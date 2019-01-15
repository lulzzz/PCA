using System;
using FluentValidation;
using FluentValidation.Results;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Features
{
    public class SetMasterKey
    {
        public class Command
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
                    .Must(baseValidator.MasterKeyDoesExists)
                    .WithMessage("'Master key' does not exists.")
                    .Must(baseValidator.MasterKeySucessfulyDecrypted)
                    .WithMessage("'Current Password' is invalid.");
                RuleFor(c => c.Password).NotEmpty().WithMessage("'Password' field is required.");
                RuleFor(c => c.RetypePassword)
                    .NotEmpty()
                    .WithMessage("'Retype Password' field is required.")
                    .Equal(c => c.Password)
                    .WithMessage("Password fields mismatch.");
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

                var passphrase = privateCertRepository.GetPassphrase();
                var passphraseDecrypted = StringCipher.Decrypt(passphrase, command.CurrentPassword);
                var passphraseEncrypted = StringCipher.Encrypt(passphraseDecrypted, command.Password);
                privateCertRepository.SetPassphrase(passphraseEncrypted);

                unitOfWork.SaveChanges();
                unitOfWork.CommitTransaction();
                return result;
            }
        }

      
    }
}