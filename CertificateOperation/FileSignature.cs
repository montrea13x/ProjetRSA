using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ProjetRSA;

namespace ProjetRSA.CertificateOperations;

public class FileSignature
{
    /// <summary>
    /// Signs a file using the RSA private key and saves the signature to a file.
    /// </summary>
    /// <param name="privateKeyFile">Path to the encrypted RSA private key file.</param>
    /// <param name="inputFile">Path to the file to be signed.</param>
    /// <param name="signatureFile">Path where the signature will be saved.</param>
    public static void SignFile(
        string privateKeyFile = "rsa_private.enc",
        string inputFile = "test.txt",
        string signatureFile = "test.sig"
        )
    {
        try
        {
            if (!File.Exists(privateKeyFile))
            {
                Loggers.LogError($"Private key file {privateKeyFile} not found.");
                throw new CryptoOperationException($"Private key file {privateKeyFile} not found.");
            }

            if (!File.Exists(inputFile))
            {
                Loggers.LogError($"File to sign ({inputFile}) not found.");
                throw new CryptoOperationException($"File to sign ({inputFile}) not found.");
            }

            using RSA? rsa = ProjetRSA.KeyOperations.KeyLoader.TryLoadPrivateKey(privateKeyFile);
            if (rsa == null)
            {
                Loggers.LogError("Failed to load private key. Cannot sign file.");
                throw new CryptoOperationException("Failed to load private key. Cannot sign file.");
            }

            byte[] data = File.ReadAllBytes(inputFile);
            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            File.WriteAllText(signatureFile, Convert.ToBase64String(signature));

            Loggers.LogInfo("File signed successfully.");
            Loggers.LogInfo($"Signature file: {signatureFile}");
        }
        catch (Exception ex)
        {
            if (ex is CryptoOperationException) throw;
            throw new CryptoOperationException("An error occurred while signing the file.", ex);
        }
    }

    public static bool VerifyFile(
        string certFile = "certificate.pem",
        string inputFile = "test.txt",
        string signatureFile = "test.sig"
        )
    {
        try
        {
            if (!File.Exists(certFile))
            {
                Loggers.LogError($"Certificate file {certFile} not found.");
                throw new CryptoOperationException($"Certificate file {certFile} not found.{Environment.NewLine} Generate a certificate first: dotnet run generatecert");
            }

            if (!File.Exists(inputFile))
            {
                Loggers.LogError($"File to verify ({inputFile}) not found.");
                throw new CryptoOperationException($"File to verify ({inputFile}) not found.");
            }

            if (!File.Exists(signatureFile))
            {
                Loggers.LogError($"Signature file ({signatureFile}) not found.");
                throw new CryptoOperationException($"Signature file ({signatureFile}) not found.");
            }

            byte[] data = File.ReadAllBytes(inputFile);
            string signatureBase64 = File.ReadAllText(signatureFile).Trim();

            byte[] signature;
            try
            {
                signature = Convert.FromBase64String(signatureBase64);
            }
            catch (FormatException ex)
            {
                throw new CryptoOperationException("Invalid signature format. Expected Base64.", ex);

            }

            using var cert = X509Certificate2.CreateFromPem(File.ReadAllText(certFile));
            using RSA? rsa = cert.GetRSAPublicKey();

            if (rsa == null)
            {
                throw new CryptoOperationException("Certificate does not contain an RSA public key.");
            }

            bool isValid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            Loggers.LogInfo(isValid ? "Signature is valid." : "Signature is invalid.");
            return isValid;
        }
        catch (Exception ex)
        {
            if (ex is CryptoOperationException) throw;
            throw new CryptoOperationException("An error occurred while verifying the signature.", ex);
        }
    }
}
