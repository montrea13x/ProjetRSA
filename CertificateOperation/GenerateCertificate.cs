using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace ProjetRSA.CertificateOperations;

public static class GenerateCertificate
{
    /// <summary>
    /// Generates a self-signed Root CA certificate and writes it to disk.
    /// </summary>
    /// <param name="certFile">Output path for the Root CA certificate.</param>
    /// <param name="keyFile">Output path for the Root CA private key.</param>
    public static void GenerateRootCAFiles(
        string certFile = "certificates/rootca.pem",
        string keyFile = "certificates/rootca.key.pem"
        )
    {
        var (cert, key) = GenerateRootCA();
        try
        {
            Loggers.LogInfo("Generating Root CA certificate...");
            File.WriteAllText(certFile, cert.ExportCertificatePem());
            Loggers.LogInfo($"Root CA certificate written to {certFile}");
            Console.WriteLine($"Root CA certificate written to {certFile}");
        }
        catch (Exception ex)
        {
            Loggers.LogError("Failed to write Root CA files.");
            throw new CertificateException("Failed to write Root CA files.", ex);
        }
        
        try
        {
            Loggers.LogInfo("Generating Root CA private key...");
            File.WriteAllText(keyFile, key.ExportPkcs8PrivateKeyPem());
            Loggers.LogInfo($"Root CA private key written to {keyFile}");
            Console.WriteLine($"Root CA private key written to {keyFile}");
        }
        catch (Exception ex)
        {
            Loggers.LogError("Failed to write Root CA private key.");
            throw new CertificateException("Failed to write Root CA private key.", ex);
        }

    }

    /// <summary>
    /// Generates an Intermediate CA certificate signed by the Root CA and writes it to disk.
    /// </summary>
    /// <param name="rootCertFile">Path to the Root CA certificate.</param>
    /// <param name="rootKeyFile">Path to the Root CA private key.</param>
    /// <param name="certFile">Output path for the Intermediate CA certificate.</param>
    /// <param name="keyFile">Output path for the Intermediate CA private key.</param>
    public static void GenerateIntermediateCAFiles(
        string rootCertFile = "certificates/rootca.pem",
        string rootKeyFile = "certificates/rootca.key.pem",
        string certFile = "certificates/intermediate.pem",
        string keyFile = "certificates/intermediate.key.pem"
        )
    {
        if (!File.Exists(rootCertFile) || !File.Exists(rootKeyFile))
        {
            Loggers.LogError("Root CA files not found. Generate them first: dotnet run generaterootcert");
            throw new CertificateException("Root CA files not found. Generate them first: dotnet run generaterootcert");
        }

        using var rootCert = X509Certificate2.CreateFromPem(File.ReadAllText(rootCertFile));
        using RSA rootKey = RSA.Create();
        rootKey.ImportFromPem(File.ReadAllText(rootKeyFile));

        var (cert, key) = GenerateIntermediateCA(rootCert, rootKey);
        try
        {
            Loggers.LogInfo("Generating Intermediate CA certificate...");
            File.WriteAllText(certFile, cert.ExportCertificatePem());
            Loggers.LogInfo($"Intermediate CA certificate written to {certFile}");
            Console.WriteLine($"Intermediate CA certificate written to {certFile}");
        }
        catch (Exception ex)
        {
            Loggers.LogError("Failed to write Intermediate CA certificate.");
            throw new CertificateException("Failed to write Intermediate CA files.", ex);
        }

        try
        {
            Loggers.LogInfo("Generating Intermediate CA private key...");
            File.WriteAllText(keyFile, key.ExportPkcs8PrivateKeyPem());
            Loggers.LogInfo($"Intermediate CA private key written to {keyFile}");
            Console.WriteLine($"Intermediate CA private key written to {keyFile}");
        }
        catch (Exception ex)
        {
            Loggers.LogError("Failed to write Intermediate CA private key.");
            throw new CertificateException("Failed to write Intermediate CA private key.", ex);
        }
    }


    /// <summary>
    /// Generates an end-entity certificate signed by the Intermediate CA and writes it to disk.
    /// </summary>
    /// <param name="intermediateCertFile">Path to the Intermediate CA certificate.</param>
    /// <param name="intermediateKeyFile">Path to the Intermediate CA private key.</param>
    /// <param name="certFile">Output path for the end-entity certificate.</param>
    /// <param name="keyFile">Output path for the end-entity private key.</param>
    public static void GenerateEndEntityCertFiles(
        string intermediateCertFile = "certificates/intermediate.pem",
        string intermediateKeyFile = "certificates/intermediate.key.pem",
        string certFile = "certificates/endentity.pem",
        string keyFile = "certificates/endentity.key.pem"
        )
    {
        if (!File.Exists(intermediateCertFile) || !File.Exists(intermediateKeyFile))
        {
            Loggers.LogError("Intermediate CA files not found. Generate them first: dotnet run generateintermediatecert");
            throw new CertificateException("Intermediate CA files not found. Generate them first: dotnet run generateintermediatecert");
        }

        using var intermediateCert = X509Certificate2.CreateFromPem(File.ReadAllText(intermediateCertFile));
        using RSA intermediateKey = RSA.Create();
        intermediateKey.ImportFromPem(File.ReadAllText(intermediateKeyFile));

        using RSA endEntityKey = RSA.Create(2048);
        X509Certificate2 cert = GenerateEndEntityCert(intermediateCert, intermediateKey, endEntityKey);
       
        try
        {
            Loggers.LogInfo("Generating end-entity certificate...");
            File.WriteAllText(certFile, cert.ExportCertificatePem());
            Loggers.LogInfo($"End-entity certificate written to {certFile}");
            Console.WriteLine($"End-entity certificate written to {certFile}");
        }
        catch (Exception ex)
        {
            Loggers.LogError("Failed to write end-entity certificate files.");
            throw new CertificateException("Failed to write end-entity certificate files.", ex);
        }

        try
        {
            Loggers.LogInfo("Generating end-entity private key...");
            File.WriteAllText(keyFile, endEntityKey.ExportPkcs8PrivateKeyPem());
            Loggers.LogInfo($"End-entity private key written to {keyFile}");
            Console.WriteLine($"End-entity private key written to {keyFile}");
        }
        catch (Exception ex)
        {
            Loggers.LogError("Failed to write end-entity private key.");
            throw new CertificateException("Failed to write end-entity private key.", ex);
        }
    }


    /// <summary>
    /// Creates a self-signed Root CA certificate in memory.
    /// </summary>
    /// <returns>The Root CA certificate and its RSA private key.</returns>
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

    /// <summary>
    /// Creates an Intermediate CA certificate signed by the provided Root CA.
    /// </summary>
    /// <param name="rootCert">Root CA certificate.</param>
    /// <param name="rootKey">Root CA private key.</param>
    /// <returns>The Intermediate CA certificate and its RSA private key.</returns>
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

    /// <summary>
    /// Creates an end-entity certificate signed by the Intermediate CA.
    /// </summary>
    /// <param name="intermediateCert">Intermediate CA certificate.</param>
    /// <param name="intermediateKey">Intermediate CA private key.</param>
    /// <param name="endEntityKey">End-entity RSA key pair.</param>
    /// <returns>The end-entity certificate with the private key attached.</returns>
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