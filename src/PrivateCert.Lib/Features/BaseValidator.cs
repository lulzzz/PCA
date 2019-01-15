using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Features
{
    public class BaseValidator
    {
        private readonly IPrivateCertRepository privateCertRepository;
        public BaseValidator(IPrivateCertRepository privateCertRepository)
        {
            this.privateCertRepository = privateCertRepository;
        }

        public bool MasterKeySucessfulyDecrypted(string password)
        {
            var masterKey = privateCertRepository.GetMasterKey();
            var passwordHashed = StringHash.GetHash(password);
            var passwordHashedString = StringHash.GetHashString(passwordHashed);
            return masterKey == passwordHashedString;
        }

        public bool MasterKeyDoesExists(object entity)
        {
            var masterKey = privateCertRepository.GetMasterKey();
            return masterKey != null;
        }

        public void MasterKeyDoesNotExists(object entity, CustomContext customContext)
        {
            if (MasterKeyDoesExists(entity))
            {
                customContext.AddFailure("Master key already exists.");
            }
        }
    }
}
