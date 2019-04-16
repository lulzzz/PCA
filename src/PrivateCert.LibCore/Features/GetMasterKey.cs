using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace PrivateCert.LibCore.Features
{
    public class GetMasterKey
    {
        public class Query : IRequest<ValidationResult>
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
                RuleFor(c => c.Password).MustAsync(baseValidator.MasterKeySucessfulyDecrypted).WithMessage("Wrong password.");
            }
        }

        public class QueryHandler : IRequestHandler<Query, ValidationResult>
        {
            private readonly QueryValidator queryValidator;

            public QueryHandler(QueryValidator queryValidator)
            {
                this.queryValidator = queryValidator;
            }

            public async Task<ValidationResult> Handle(Query request, CancellationToken cancellationToken)
            {
                return await queryValidator.ValidateAsync(request, cancellationToken);
            }
        }
    }
}
