using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using PrivateCert.Lib.Features;
using PrivateCert.Lib.Interfaces;

namespace PrivateCert.Lib.Model
{
    public class Certificate
    {
        public static Certificate CreateRootCertificate(CreateRootCertificate.Command command, string passphraseDecrypted)
        {
            var rootDN = $"C={command.Country},O={command.Organization},OU={command.OrganizationUnit},CN=" + command.SubjectName.Trim();
            var x509 = GenerateRootCertificate(rootDN, command.CRLs);
            var certificate = new Certificate()
            {
                ExpirationDate = x509.NotAfter,
                CertificateType = CertificateTypeEnum.Root,
                SerialNumber = x509.SerialNumber,
                Name = x509.GetNameInfo(X509NameType.SimpleName, false),
                Thumbprint = x509.Thumbprint,
                IssueDate = x509.NotBefore,
            };

            certificate.PfxData = x509.Export(X509ContentType.Pfx, passphraseDecrypted);
            return certificate;
        }

        public int CertificateId { get;  private set;}

        public DateTime ExpirationDate { get; private set; }

        public CertificateTypeEnum CertificateType
        {
            get => (CertificateTypeEnum) CertificateTypeId;
            set => CertificateTypeId = (byte) value;
        }

        public byte CertificateTypeId { get; private set; }

        public string SerialNumber { get; private set; }

        public string Name { get; private set;}

        public string Thumbprint { get; private set;}

        public DateTime IssueDate { get; private set;}

        public DateTime? RevocationDate { get; private set;}

        public byte[] PfxData { get; private set;}

        private static X509Certificate2 GenerateRootCertificate(string subjectName, ICollection<string> crlUrls, int keyStrength = 2048)
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
            AddRevocationUrls(certificateGenerator, crlUrls);

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

        public static X509Certificate2 CreateServerCertificate(CreateServerCertificate.Command command, X509Certificate2 rootCertificate, string passphraseDecrypted)
        {
            // Geração do certificado
            var name = command.IssuerName.Trim();

            var userDN = $"{rootCertificate.GetNameInfo(X509NameType.DnsName, false)},CN=" + name;
            var userRepository = new UserRepository(context);
            var p7bUrlPrefixes = userRepository.GetDbSettingString("P7bUrlPrefixes").Split('|');
        }

        private static X509Certificate2 GenerateSslCertificate(string subjectName, DateTime now, DateTime expirationDate, X509Certificate2 issuerCertificate, int versaoCertificadoRaiz, string[] p7bUrlPrefixes, string siteName, int keyStrength = 2048)
        {
            var issuerPrivKey = DotNetUtilities.GetKeyPair(issuerCertificate.PrivateKey).Private;
            var issuerPublicKey = DotNetUtilities.GetKeyPair(issuerCertificate.PrivateKey).Public;

            // Generating Random Numbers
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            // The Certificate Generator
            var certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            // Issuer and Subject Name
            var subjectDN = new X509Name(subjectName);

            // O nome do emissor precisa ser invertido. Por algum motivo estranho, o nome do emissor ficar invertido ao nome do emissor no certificado raiz. 
            // O aplicativo certutil acaba reclamando disso. Ao inverter o nome, os dois ficam corretos no programa e ele não reclama. 
            // Como isso pode ser um ponto de problema, achei melhor fazer essa adaptação. (Paulo)
            var issuerName = issuerCertificate.IssuerName.Name;
            var issuerNameElements = issuerName.Split(',');
            Array.Reverse(issuerNameElements);
            var issuerDN = new X509Name(string.Join(",", issuerNameElements));
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Valid For
            certificateGenerator.SetNotBefore(now.ToUniversalTime());
            certificateGenerator.SetNotAfter(expirationDate.ToUniversalTime());

            // Subject Public Key
            var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            // Informações adicionais para encontrar a cadeia.
            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(issuerPublicKey));
            certificateGenerator.AddExtension(
                X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(subjectKeyPair.Public));

            // Adiciona caminhos de CRL do certificado raiz
            AddRevocationUrlsFromIssuer(certificateGenerator, issuerCertificate);

            AddP7bUrls(versaoCertificadoRaiz, certificateGenerator, p7bUrlPrefixes);

            // Informações adicionais para definir a finalidade do certificado.
            certificateGenerator.AddExtension(
                X509Extensions.KeyUsage, true,
                new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment | KeyUsage.NonRepudiation));

            certificateGenerator.AddExtension(
                X509Extensions.ExtendedKeyUsage, false,
                new ExtendedKeyUsage(KeyPurposeID.IdKPServerAuth, KeyPurposeID.IdKPClientAuth));

            certificateGenerator.AddExtension(
                X509Extensions.SubjectAlternativeName, false,
                new GeneralNames(new GeneralName(GeneralName.DnsName, siteName)));

            certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(false));

            // SSL Certificate 
            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", issuerPrivKey, random);
            var certificate = certificateGenerator.Generate(signatureFactory);

            // correcponding private key
            var info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);

            // merge into X509Certificate2
            var x509 = new X509Certificate2(certificate.GetEncoded());

            var seq = (Asn1Sequence)Asn1Object.FromByteArray(info.ParsePrivateKey().GetDerEncoded());
            if (seq.Count != 9)
            {
                throw new PemException("malformed sequence in RSA private key");
            }

            var rsa = RsaPrivateKeyStructure.GetInstance(seq);
            var rsaparams = new RsaPrivateCrtKeyParameters(
                rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent, rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2,
                rsa.Coefficient);

            x509.PrivateKey = DotNetUtilities.ToRSA(rsaparams);
            x509.FriendlyName = subjectDN.GetValueList()[subjectDN.GetValueList().Count - 1].ToString();
            return x509;
        }


            public static X509Certificate2 GenerateClientCertificate(
                string subjectName, DateTime now, DateTime expirationDate, X509Certificate2 issuerCertificate,
                int versaoCertificadoRaiz, string[] p7bUrlPrefixes, string upnName, int keyStrength = 2048)
            {
                var issuerPrivKey = DotNetUtilities.GetKeyPair(issuerCertificate.PrivateKey).Private;
                var issuerPublicKey = DotNetUtilities.GetKeyPair(issuerCertificate.PrivateKey).Public;

                // Generating Random Numbers
                var randomGenerator = new CryptoApiRandomGenerator();
                var random = new SecureRandom(randomGenerator);

                // The Certificate Generator
                var certificateGenerator = new X509V3CertificateGenerator();

                // Serial Number
                var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
                certificateGenerator.SetSerialNumber(serialNumber);

                // Issuer and Subject Name
                var subjectDN = new X509Name(subjectName);

                // O nome do emissor precisa ser invertido. Por algum motivo estranho, o nome do emissor ficar invertido ao nome do emissor no certificado raiz. 
                // O aplicativo certutil acaba reclamando disso. Ao inverter o nome, os dois ficam corretos no programa e ele não reclama. 
                // Como isso pode ser um ponto de problema, achei melhor fazer essa adaptação. (Paulo)
                var issuerName = issuerCertificate.IssuerName.Name;
                var issuerNameElements = issuerName.Split(',');
                Array.Reverse(issuerNameElements);
                var issuerDN = new X509Name(string.Join(",", issuerNameElements));
                certificateGenerator.SetIssuerDN(issuerDN);
                certificateGenerator.SetSubjectDN(subjectDN);

                // Valid For
                certificateGenerator.SetNotBefore(now.ToUniversalTime());
                certificateGenerator.SetNotAfter(expirationDate.ToUniversalTime());

                // Subject Public Key
                var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
                var keyPairGenerator = new RsaKeyPairGenerator();
                keyPairGenerator.Init(keyGenerationParameters);
                var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

                certificateGenerator.SetPublicKey(subjectKeyPair.Public);

                // Informações adicionais para encontrar a cadeia.
                certificateGenerator.AddExtension(
                    X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(issuerPublicKey));
                certificateGenerator.AddExtension(
                    X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(subjectKeyPair.Public));

                // Adiciona caminhos de CRL do certificado raiz
                AddRevocationUrlsFromIssuer(certificateGenerator, issuerCertificate);

                AddP7bUrls(versaoCertificadoRaiz, certificateGenerator, p7bUrlPrefixes);

                // Informações adicionais para definir a finalidade do certificado.
                certificateGenerator.AddExtension(
                    X509Extensions.KeyUsage, true,
                    new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment | KeyUsage.NonRepudiation));

                certificateGenerator.AddExtension(
                    X509Extensions.ExtendedKeyUsage, false,
                    new ExtendedKeyUsage(
                        KeyPurposeID.IdKPClientAuth, KeyPurposeID.IdKPSmartCardLogon, KeyPurposeID.IdKPCodeSigning));

                Asn1EncodableVector otherName = new Asn1EncodableVector();
                otherName.Add(new DerObjectIdentifier("1.3.6.1.4.1.311.20.2.3"));
                otherName.Add(new DerTaggedObject(true, GeneralName.OtherName, new DerUtf8String(upnName)));
                Asn1Object upn = new DerTaggedObject(false, 0, new DerSequence(otherName));
                Asn1EncodableVector generalNames = new Asn1EncodableVector();
                generalNames.Add(upn);

                // Adding extension to X509V3CertificateGenerator
                certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName, false, new DerSequence(generalNames));

                certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(false));

                // SSL Certificate 
                ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", issuerPrivKey, random);
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
                x509.FriendlyName = subjectDN.GetValueList()[subjectDN.GetValueList().Count - 1].ToString();
                return x509;
            }

        private static void AddP7bUrls(int versao, X509V3CertificateGenerator certificateGenerator, string[] p7bUrlPrefixes)
        {
            var versionInfo = string.Format("/v{0}/chain.p7b", versao);
            var descriptions = new List<AccessDescription>();

            int count = 1;

            foreach (var certificatesPrefix in p7bUrlPrefixes)
            {
                // X509Chain only validates up to 2 urls. Então não adianta colocar mais do que duas.
                if (count > 2)
                {
                    break;
                }

                var description = new AccessDescription(
                    X509ObjectIdentifiers.IdADCAIssuers,
                    new GeneralName(GeneralName.UniformResourceIdentifier, certificatesPrefix + versionInfo));
                descriptions.Add(description);

                count ++;
            }

            certificateGenerator.AddExtension(X509Extensions.AuthorityInfoAccess, false, new DerSequence(descriptions.ToArray()));
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

        private static void AddRevocationUrls(X509V3CertificateGenerator certificateGenerator, ICollection<string> urlCrls)
        {
            //var versionInfo = string.Format("/v{0}/rootCertificate.crl", versao);
            var distPoints = new List<DistributionPoint>();
            foreach (var urlCrl in urlCrls)
            {
                var distPoint = new DistributionPointName(
                    new GeneralNames(new GeneralName(GeneralName.UniformResourceIdentifier, urlCrl)));
                distPoints.Add(new DistributionPoint(distPoint, null, null));
            }

            certificateGenerator.AddExtension(
                X509Extensions.CrlDistributionPoints, false, new CrlDistPoint(distPoints.ToArray()));
        }
    }
}
