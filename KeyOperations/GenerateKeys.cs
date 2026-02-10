using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;

namespace ProjetRSA.KeyOperations;

public static class GenerateKeys
{
    public static void Execute(string[] args)
    {
        int keySize = Program.ParseKeySize(args.Skip(1).ToArray());

        try
        {
            using var rsa = RSA.Create(keySize);

            string privateKeyPem = new string(
                PemEncoding.Write("PRIVATE KEY", rsa.ExportPkcs8PrivateKey())
            );

            string publicKeyPem = new string(
                PemEncoding.Write("PUBLIC KEY", rsa.ExportSubjectPublicKeyInfo())
            );

            Console.Write("Enter password to encrypt private key: ");
            string password = PasswordHelper.ReadPassword();

            Console.Write("\nConfirm password: ");
            string confirmPassword = PasswordHelper.ReadPassword();

            while (password != confirmPassword)
            {
                Loggers.LogError("Passwords do not match.");
                Console.WriteLine("\nPasswords do not match. Please try again.");

                Console.Write("Enter password to encrypt private key: ");
                password = PasswordHelper.ReadPassword();

                Console.Write("\nConfirm password: ");
                confirmPassword = PasswordHelper.ReadPassword();
            }

            byte[] encryptedPrivateKey = PrivateKeyEncryptor.Encrypt(privateKeyPem, password);
            
            password = string.Empty;
            confirmPassword = string.Empty;

            try
            {
                Loggers.LogInfo("Writing encrypted private key to rsa_private.enc...");
                File.WriteAllBytes("rsa_private.enc", encryptedPrivateKey);
                Loggers.LogInfo("Successfully wrote encrypted private key to rsa_private.enc.");
            }
            catch (Exception ex)
            {
                Loggers.LogError($"Failed to write encrypted private key: {ex.Message}");
                throw new KeyException($"Failed to write encrypted private key: {ex.Message}");
            }

            try
            {
                Loggers.LogInfo("Writing public key to rsa_public.pem...");
                File.WriteAllText("rsa_public.pem", publicKeyPem, Encoding.ASCII);
                Loggers.LogInfo("Successfully wrote public key to rsa_public.pem.");
            }
            catch (Exception ex)
            {
                Loggers.LogError($"Failed to write public key: {ex.Message}");
                throw new KeyException($"Failed to write public key: {ex.Message}");
            }

            Console.WriteLine($"\nRSA key pair generated ({keySize} bits).");
            Console.WriteLine("Saved rsa_private.enc (encrypted) and rsa_public.pem.");

        }
        catch (CryptographicException ex)
        {
            Loggers.LogError($"Cryptographic error: {ex.Message}");
            throw new KeyException($"Cryptographic error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Loggers.LogError($"Error: {ex.Message}");
            throw new KeyException($"Error: {ex.Message}");
        }
    }
}