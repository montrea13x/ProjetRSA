using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA.KeyOperations;

public static class ReadKeys
{
    public static void Execute(
        string publicKeyFile = "rsa_public.pem",
        string privateKeyFile = "rsa_private.enc")
    {
        Console.WriteLine("=== Read RSA Keys ===\n");

        try
        {
            CheckRSAKey.Execute(publicKeyFile, privateKeyFile);
        }
        catch (KeyException ex)
        {
            Loggers.LogError(ex.Message);
            throw new KeyException ($"\n{ex.Message}\nAborting.");
        }

        Console.WriteLine("--- PUBLIC KEY ---");
        try
        {
            string publicKey = File.ReadAllText(publicKeyFile);
            Loggers.LogInfo("Public key read successfully.");
            Console.WriteLine(publicKey);
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Loggers.LogError($"Error: Failed to read public key. {ex.Message}");
            throw new KeyException($"\nError: Failed to read public key. {ex.Message}");
        }


        // Read and decrypt private key
        Console.WriteLine("--- PRIVATE KEY (Encrypted) ---");
        Console.Write("Enter password to decrypt private key: ");
        string password = PasswordHelper.ReadPassword();

        try
        {
            byte[] encryptedData = File.ReadAllBytes(privateKeyFile);
            string privateKey = PrivateKeyEncryptor.Decrypt(encryptedData, password);

            Loggers.LogInfo("Private key decrypted successfully.");
            Console.WriteLine("\n--- PRIVATE KEY (Decrypted) ---");
            Console.WriteLine(privateKey);
        }

        catch (Exception ex)
        {
            Loggers.LogError($"Error: Failed to decrypt private key. {ex.Message}");
            throw new KeyException($"\nError: Failed to decrypt private key. {ex.Message}");
        }

        finally
        {
            password = string.Empty;
        }
    }
}