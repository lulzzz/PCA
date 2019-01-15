using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Model
{
    public class Certificate
    {
        public static Certificate CreateRootCertificate(DateTime expirationDate, string country, string organization, string organizationUnit, string name, DateTime issueDate, string[] crlUrl)
        {
            var rootDN = $"C={country},O={organization},OU={organizationUnit},CN=" + name.Trim();
            var x509 = GenerateRootCertificate(rootDN, crlUrl);
            var certificate = new Certificate()
            {
                ExpirationDate = x509.NotAfter,
                CertificateType = CertificateTypeEnum.Root,
                SerialNumber = x509.SerialNumber,
                Name = x509.GetNameInfo(X509NameType.SimpleName, false),
                Thumbprint = x509.Thumbprint,
                IssueDate = x509.NotBefore//PfxPassword = 
            };

            return certificate;
        }

        public int CertificateId { get;  private set;}

        public DateTime ExpirationDate { get; private set; }

        public CertificateTypeEnum CertificateType { get; private set;}

        public string SerialNumber { get; private set; }

        public string Name { get; private set;}

        public string Thumbprint { get; private set;}

        public DateTime IssueDate { get; private set;}

        public DateTime? RevocationDate { get; private set;}

        public byte[] PfxData { get; private set;}

        public string PfxPassword { get; private set;}

        public static X509Certificate2 GenerateRootCertificate(string subjectName, string[] crlUrlPrefixes, int keyStrength = 2048)
        {
            // Generating Random Numbers
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            // The Certificate Generator
            var certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            var serialNumber = BigInteger.One;

            // TODO: colocar um ID qualquer que controle a tabela de certificados.
            certificateGenerator.SetSerialNumber(serialNumber);

            // Issuer and Subject Name
            var subjectDN = new X509Name(subjectName);
            var issuerDN = new X509Name(subjectName);
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            var notBefore = DateTime.UtcNow;
            var notAfter = notBefore.AddYears(20);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // Subject Public Key
            var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            // Define que pode assinar certificados
            var x = new KeyUsage(KeyUsage.KeyCertSign | KeyUsage.CrlSign | KeyUsage.DigitalSignature);
            certificateGenerator.AddExtension(X509Extensions.KeyUsage, true, x);

            // Need this extension to signify that this certificate is a CA and
            // can issue certificates. (Extension is marked as critical)
            certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(true));

            // Adiciona caminhos de CRL
            AddRevocationUrls(certificateGenerator, crlUrlPrefixes);

            // Informações adicionais para encontrar a cadeia.
            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(subjectKeyPair.Public));

            certificateGenerator.AddExtension(
                X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(subjectKeyPair.Public));

            // Generating the Certificate
            var issuerKeyPair = subjectKeyPair;
            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", issuerKeyPair.Private, random);

            // selfsign certificate
            var certificate = certificateGenerator.Generate(signatureFactory);

            // correcponding private key
            var info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);

            // merge into X509Certificate2
            var x509 = new X509Certificate2(certificate.GetEncoded());

            var seq = (Asn1Sequence) Asn1Object.FromByteArray(info.ParsePrivateKey().GetDerEncoded());
            if (seq.Count != 9)
            {
                throw new PemException("malformed sequence in RSA private key");
            }

            var rsa = RsaPrivateKeyStructure.GetInstance(seq);
            var rsaparams = new RsaPrivateCrtKeyParameters(
                rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent, rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2,
                rsa.Coefficient);

            x509.PrivateKey = DotNetUtilities.ToRSA(rsaparams);
            return x509;
        }

        private static void AddRevocationUrls(X509V3CertificateGenerator certificateGenerator, string[] crlPrefixes)
        {
            //var versionInfo = string.Format("/v{0}/rootCertificate.crl", versao);
            var distPoints = new List<DistributionPoint>();
            foreach (var certificatesPrefix in crlPrefixes)
            {
                var distPoint = new DistributionPointName(
                    new GeneralNames(new GeneralName(GeneralName.UniformResourceIdentifier, certificatesPrefix)));
                distPoints.Add(new DistributionPoint(distPoint, null, null));
            }

            certificateGenerator.AddExtension(
                X509Extensions.CrlDistributionPoints, false, new CrlDistPoint(distPoints.ToArray()));
        }
    }
}
