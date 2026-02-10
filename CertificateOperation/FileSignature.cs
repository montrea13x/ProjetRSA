using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProjetRSA.CertificateOperations;

class FileSignature
{
    public static void SignFile(
        string privateKeyFile = "rsa_private.enc",
        string inputFile = "test.txt",
        string signatureFile = "test.sig"
        )
    {
        if (!File.Exists(privateKeyFile))
        {
            Loggers.LogError($"Private key file {privateKeyFile} not found.");
            Console.WriteLine($"Private key file {privateKeyFile} not found.");
            return;
        }

        if (!File.Exists(inputFile))
        {
            Loggers.LogError($"File to sign ({inputFile}) not found.");
            Console.WriteLine($"File to sign ({inputFile}) not found.");
            return;
        }

        using RSA? rsa = ProjetRSA.KeyOperations.KeyLoader.TryLoadPrivateKey(privateKeyFile);
        if (rsa == null)
        {
            Loggers.LogError("Failed to load private key. Cannot sign file.");
            Console.WriteLine("Failed to load private key. Cannot sign file.");
            return;
        }

        byte[] data = File.ReadAllBytes(inputFile);
        byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        File.WriteAllText(signatureFile, Convert.ToBase64String(signature));
        
        Loggers.LogInfo("File signed successfully.");
        Loggers.LogInfo($"Signature file: {signatureFile}");
    }

    public static void VerifyFile(
        string certFile = "certificate.pem",
        string inputFile = "test.txt",
        string signatureFile = "test.sig"
        )
    {
        if (!File.Exists(certFile))
        {
            Loggers.LogError($"Certificate file {certFile} not found.");
            throw new CertificateException($"Certificate file {certFile} not found.{Environment.NewLine} Generate a certificate first: dotnet run generatecert");
        }

        if (!File.Exists(inputFile))
        {
            Loggers.LogError($"File to verify ({inputFile}) not found.");
            throw new CertificateException($"File to verify ({inputFile}) not found.");
        }

        if (!File.Exists(signatureFile))
        {
            Loggers.LogError($"Signature file ({signatureFile}) not found.");
            throw new CertificateException($"Signature file ({signatureFile}) not found.");
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
            throw new CertificateException("Invalid signature format. Expected Base64.");

        }

        using var cert = X509Certificate2.CreateFromPem(File.ReadAllText(certFile));
        using RSA? rsa = cert.GetRSAPublicKey();

        if (rsa == null)
        {
            throw new CertificateException("Certificate does not contain an RSA public key.");
        }

        bool isValid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        Loggers.LogInfo(isValid ? "Signature is valid." : "Signature is invalid.");
    }
}
