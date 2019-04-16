using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Validators;
using PrivateCert.LibCore.Infrastructure;
using PrivateCert.LibCore.Interfaces;

namespace PrivateCert.LibCore.Features
{
    public class BaseValidator
    {
        private readonly IPrivateCertRepository privateCertRepository;
        public BaseValidator(IPrivateCertRepository privateCertRepository)
        {
            this.privateCertRepository = privateCertRepository;
        }

        public async Task<bool> MasterKeySucessfulyDecrypted(string password, CancellationToken cancellationToken)
        {
            var masterKey = await privateCertRepository.GetMasterKeyAsync();
            var passwordHashed = StringHash.GetHash(password);
            var passwordHashedString = StringHash.GetHashString(passwordHashed);
            return masterKey == passwordHashedString;
        }

        public async Task<bool> MasterKeyDoesExists(object entity, CancellationToken cancellationToken)
        {
            var masterKey = await privateCertRepository.GetMasterKeyAsync();
            return masterKey != null;
        }

        public async Task MasterKeyDoesNotExists(object entity, CustomContext customContext, CancellationToken cancellationToken)
        {
            if (await MasterKeyDoesExists(entity, cancellationToken))
            {
                customContext.AddFailure("Master key already exists.");
            }
        }
    }
}
