using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProjetRSA.CertificateOperations;

class Signature
{
    public static void SignFile(
        string privateKeyFile = "rsa_private.enc",
        string inputFile = "test.txt",
        string signatureFile = "test.sig"
        )
    {
        if (!File.Exists(privateKeyFile))
        {
            Console.WriteLine($"Private key file {privateKeyFile} not found.");
            return;
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"File to sign ({inputFile}) not found.");
            return;
        }

        using RSA? rsa = ProjetRSA.KeyOperations.KeyLoader.TryLoadPrivateKey(privateKeyFile);
        if (rsa == null)
        {
            return;
        }

        byte[] data = File.ReadAllBytes(inputFile);
        byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        File.WriteAllText(signatureFile, Convert.ToBase64String(signature));

        Console.WriteLine("File signed successfully.");
        Console.WriteLine($"Signature file: {signatureFile}");
    }

    public static void VerifyFile(
        string certFile = "certificate.pem",
        string inputFile = "test.txt",
        string signatureFile = "test.sig"
        )
    {
        if (!File.Exists(certFile))
        {
            Console.WriteLine($"Certificate file {certFile} not found.");
            Console.WriteLine("Generate a certificate first: dotnet run generatecert");
            return;
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"File to verify ({inputFile}) not found.");
            return;
        }

        if (!File.Exists(signatureFile))
        {
            Console.WriteLine($"Signature file ({signatureFile}) not found.");
            return;
        }

        byte[] data = File.ReadAllBytes(inputFile);
        string signatureBase64 = File.ReadAllText(signatureFile).Trim();

        byte[] signature;
        try
        {
            signature = Convert.FromBase64String(signatureBase64);
        }
        catch (FormatException)
        {
            throw new CertificateOperation.CertificateException("Invalid signature format. Expected Base64.");

        }

        using var cert = X509Certificate2.CreateFromPem(File.ReadAllText(certFile));
        using RSA? rsa = cert.GetRSAPublicKey();

        if (rsa == null)
        {
            throw new CertificateOperation.CertificateException("Certificate does not contain an RSA public key.");
        }

        bool isValid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        Console.WriteLine(isValid ? "Signature is valid." : "Signature is invalid.");
    }
}
