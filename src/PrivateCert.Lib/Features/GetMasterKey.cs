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
    public class GetMasterKey
    {
        public class Query
        {
            public Query(string password)
            {
                Password = password;
            }

            public string Password { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(BaseValidator baseValidator)
            {
                RuleFor(c => c.Password).Must(baseValidator.MasterKeySucessfulyDecrypted).WithMessage("Wrong password.");
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
    }
}
