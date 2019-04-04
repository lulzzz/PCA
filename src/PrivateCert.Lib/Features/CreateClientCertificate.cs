using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Features
{
    public class CreateClientCertificate
    {
        public class Query
        {
        }

        public class ViewModel : IValidationResult
        {
            public string Domain { get; set; }

            public int ExpirationDateInDays { get; set; }

            public string IssuerName { get; set; }

            public ValidationResult ValidationResult { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
        }

        public class QueryHandler
        {
            private readonly QueryValidator queryValidator;

            public QueryHandler(QueryValidator queryValidator)
            {
                this.queryValidator = queryValidator;
            }

            public ViewModel Handle(Query query)
            {
                var viewModel = new ViewModel();
                viewModel.ValidationResult = queryValidator.Validate(query);
                if (!viewModel.ValidationResult.IsValid)
                {
                    return viewModel;
                }

                return viewModel;
            }
        }

        public class Command : IRequest<ValidationResult>
        {
            public Command(ViewModel viewModel, string masterKeyDecrypted)
            {
                ViewModel = viewModel;
                MasterKeyDecrypted = masterKeyDecrypted;
            }

            public string MasterKeyDecrypted { get; }

            public ViewModel ViewModel { get; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
        }

        public class CommandHandler : IRequestHandler<Command, ValidationResult>
        {
            private readonly CommandValidator commandValidator;

            public CommandHandler(CommandValidator commandValidator)
            {
                this.commandValidator = commandValidator;
            }


            private static void AddRevocationUrlsFromIssuer(
                X509V3CertificateGenerator certificateGenerator,
                X509Certificate2 issuerCertificate)
            {
                foreach (var extension in issuerCertificate.Extensions)
                {
                    if (extension.Oid.Value == new Oid(X509Extensions.CrlDistributionPoints.Id).Value)
                    {
                        var extObj = Asn1Object.FromByteArray(extension.RawData);
                        var clrDistPoint = CrlDistPoint.GetInstance(extObj);

                        certificateGenerator.AddExtension(X509Extensions.CrlDistributionPoints, false, clrDistPoint);
                    }
                }
            }

            public async Task<ValidationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var result = await commandValidator.ValidateAsync(command, cancellationToken);
                if (!result.IsValid)
                {
                    return result;
                }

                return result;
            }
        }
    }
}