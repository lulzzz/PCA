using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Lib.Model;

namespace PrivateCert.Lib.Features
{
    public class ListCertificates
    {
        public class Query
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

        public class QueryHandler
        {
            private readonly QueryValidator queryValidator;

            private readonly IPrivateCertRepository privateCertRepository;

            public QueryHandler(QueryValidator queryValidator, IPrivateCertRepository privateCertRepository)
            {
                this.queryValidator = queryValidator;
                this.privateCertRepository = privateCertRepository;
            }

            public ViewModel Handle(Query query)
            {
                var viewModel = new ViewModel();
                viewModel.ValidationResult = queryValidator.Validate(query);
                if (!viewModel.ValidationResult.IsValid)
                {
                    return viewModel;
                }

                viewModel.Certificates = AutoMapper.Mapper.Map<ICollection<CertificateVM>>(privateCertRepository.GetAllCertificates());

                return viewModel;
            }
        }
    }
}
