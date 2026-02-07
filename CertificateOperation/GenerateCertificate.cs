using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace ProjetRSA.CertificateOperations;

class Certificate
{
    public static void GenerateRootCAFiles(
        string certFile = "rootca.pem",
        string keyFile = "rootca.key.pem"
        )
    {
        var (cert, key) = GenerateRootCA();
        File.WriteAllText(certFile, cert.ExportCertificatePem());
        File.WriteAllText(keyFile, key.ExportPkcs8PrivateKeyPem());
        Console.WriteLine($"Root CA certificate written to {certFile}");
        Console.WriteLine($"Root CA private key written to {keyFile}");
    }

    public static void GenerateIntermediateCAFiles(
        string rootCertFile = "rootca.pem",
        string rootKeyFile = "rootca.key.pem",
        string certFile = "intermediate.pem",
        string keyFile = "intermediate.key.pem"
        )
    {
        if (!File.Exists(rootCertFile) || !File.Exists(rootKeyFile))
        {
            Console.WriteLine("Root CA files not found. Generate them first: dotnet run generaterootcert");
            return;
        }

        using var rootCert = X509Certificate2.CreateFromPem(File.ReadAllText(rootCertFile));
        using RSA rootKey = RSA.Create();
        rootKey.ImportFromPem(File.ReadAllText(rootKeyFile));

        var (cert, key) = GenerateIntermediateCA(rootCert, rootKey);
        File.WriteAllText(certFile, cert.ExportCertificatePem());
        File.WriteAllText(keyFile, key.ExportPkcs8PrivateKeyPem());
        Console.WriteLine($"Intermediate CA certificate written to {certFile}");
        Console.WriteLine($"Intermediate CA private key written to {keyFile}");
    }

    public static void GenerateEndEntityCertFiles(
        string intermediateCertFile = "intermediate.pem",
        string intermediateKeyFile = "intermediate.key.pem",
        string certFile = "endentity.pem",
        string keyFile = "endentity.key.pem"
        )
    {
        if (!File.Exists(intermediateCertFile) || !File.Exists(intermediateKeyFile))
        {
            Console.WriteLine("Intermediate CA files not found. Generate them first: dotnet run generateintermediatecert");
            return;
        }

        using var intermediateCert = X509Certificate2.CreateFromPem(File.ReadAllText(intermediateCertFile));
        using RSA intermediateKey = RSA.Create();
        intermediateKey.ImportFromPem(File.ReadAllText(intermediateKeyFile));

        using RSA endEntityKey = RSA.Create(2048);
        X509Certificate2 cert = GenerateEndEntityCert(intermediateCert, intermediateKey, endEntityKey);
        File.WriteAllText(certFile, cert.ExportCertificatePem());
        File.WriteAllText(keyFile, endEntityKey.ExportPkcs8PrivateKeyPem());
        Console.WriteLine($"End-entity certificate written to {certFile}");
        Console.WriteLine($"End-entity private key written to {keyFile}");
    }

    public static (X509Certificate2 cert, RSA key) GenerateRootCA()
    {
        RSA rsa = RSA.Create(4096);

        var req = new CertificateRequest(
            "CN=ProvencherRootCA, O=ProvencherCode, C=CA",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        req.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(true, false, 0, true)
        );

        req.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                true
            )
        );

        var cert = req.CreateSelfSigned(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(10)
        );

        return (cert, rsa);
    }

    public static (X509Certificate2 cert, RSA key) GenerateIntermediateCA(X509Certificate2 rootCert, RSA rootKey)
    {
        RSA rsa = RSA.Create(4096);

        X509Certificate2 issuerCert = rootCert.HasPrivateKey
            ? rootCert
            : rootCert.CopyWithPrivateKey(rootKey);

        var req = new CertificateRequest(
            "CN=ProvencherIntermediateCA, O=ProvencherCode, C=CA",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        req.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(true, false, 0, true)
        );

        req.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                true
            )
        );

        X509Certificate2 cert = req.Create(
            issuerCert,
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(5),
            RandomNumberGenerator.GetBytes(16)
        );
        X509Certificate2 certWithKey = cert.CopyWithPrivateKey(rsa);
        cert.Dispose();

        return (certWithKey, rsa);
    }

    public static X509Certificate2 GenerateEndEntityCert(X509Certificate2 intermediateCert, RSA intermediateKey, RSA endEntityKey)
    {
        X509Certificate2 issuerCert = intermediateCert.HasPrivateKey
            ? intermediateCert
            : intermediateCert.CopyWithPrivateKey(intermediateKey);

        var req = new CertificateRequest(
            "CN=ProvencherEndEntity, O=ProvencherCode, C=CA",
            endEntityKey,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        req.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, true)
        );

        req.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                true
            )
        );

        X509Certificate2 cert = req.Create(
            issuerCert,
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(2),
            RandomNumberGenerator.GetBytes(16)
        );
        X509Certificate2 certWithKey = cert.CopyWithPrivateKey(endEntityKey);
        cert.Dispose();

        return certWithKey;
    }
}