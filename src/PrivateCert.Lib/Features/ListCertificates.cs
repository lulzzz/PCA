using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Lib.Model;

namespace PrivateCert.Lib.Features
{
    public class ListCertificates
    {
        public class Query : IRequest<ViewModel>
        {

        }

        public class ViewModel : IValidationResult
        {
            public ValidationResult ValidationResult { get; set; }

            public ICollection<CertificateVM> Certificates { get; set; }
        }

        public class CertificateVM
        {
        public int CertificateId { get;  set;}

        public DateTime ExpirationDate { get; set; }

        public CertificateTypeEnum CertificateType { get; set; }

        public string SerialNumber { get;set; }

        public string Name { get;  set;}

        public string Thumbprint { get; set;}

        public DateTime IssueDate { get; set;}

        public DateTime? RevocationDate { get; set;}

        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
            }
        }

        public class QueryHandler : IRequestHandler<Query,ViewModel>
        {
            private readonly QueryValidator queryValidator;

            private readonly IPrivateCertRepository privateCertRepository;

            public QueryHandler(QueryValidator queryValidator, IPrivateCertRepository privateCertRepository)
            {
                this.queryValidator = queryValidator;
                this.privateCertRepository = privateCertRepository;
            }

            public async Task<ViewModel> Handle(Query request, CancellationToken cancellationToken)
            {
                var viewModel = new ViewModel();
                viewModel.ValidationResult = await queryValidator.ValidateAsync(request, cancellationToken);
                if (!viewModel.ValidationResult.IsValid)
                {
                    return viewModel;
                }

                var certificates = await privateCertRepository.GetAllCertificatesAsync();
                viewModel.Certificates = AutoMapper.Mapper.Map<ICollection<CertificateVM>>(certificates);

                return viewModel;                
            }
        }
    }
}
