using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace ProjetRSA.CertificateOperations;

class Certificate
{
    public static void GenerateCertificate()
    {
        if (!File.Exists("rsa_private.enc"))
        {
            Console.WriteLine("Private key file rsa_private.enc not found.");
            Console.WriteLine("Generate keys first: dotnet run generate 2048");
            return;
        }

        byte[] encryptedPrivateKey = File.ReadAllBytes("rsa_private.enc");

        Console.Write("Enter password to decrypt private key: ");
        string password = PasswordHelper.ReadPassword();

        string privateKeyPem;
        try
        {
            privateKeyPem = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Decrypt(encryptedPrivateKey, password);
        }
        catch (CryptographicException)
        {
            Console.WriteLine("Invalid password or corrupted private key.");
            return;
        }

        using RSA rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);

        var req = new CertificateRequest(
            "CN=ProvencherCertificat, O=ProvencherCode, C=CA",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        var cert = req.CreateSelfSigned(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(1)
        );

        File.WriteAllBytes("certificate.cer", cert.Export(X509ContentType.Cert));
        File.WriteAllText("certificate.pem", cert.ExportCertificatePem());
    }
}