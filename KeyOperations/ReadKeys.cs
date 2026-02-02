using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA.KeyOperations;

public static class ReadKeys
{
    public static void Execute()
    {
        Console.WriteLine("=== Read RSA Keys ===\n");

        if (!CheckRSAKey.Execute())
        {
            Console.WriteLine("\nMissing RSA key files. Aborting.");
            return;
        }
        
        Console.WriteLine("--- PUBLIC KEY ---");
        string publicKey = File.ReadAllText("rsa_public.pem");
        Console.WriteLine(publicKey);
        Console.WriteLine();
        

        // Read and decrypt private key
        Console.WriteLine("--- PRIVATE KEY (Encrypted) ---");
        Console.Write("Enter password to decrypt private key: ");
        string password = PasswordHelper.ReadPassword();

        try
        {
            byte[] encryptedData = File.ReadAllBytes("rsa_private.enc");
            string privateKey = PrivateKeyEncryptor.Decrypt(encryptedData, password);

            Console.WriteLine("\n--- PRIVATE KEY (Decrypted) ---");
            Console.WriteLine(privateKey);
        }

        catch (Exception ex)
        {
            Console.WriteLine($"\nError: Failed to decrypt private key. {ex.Message}");
        }

        finally
        {
            password = string.Empty;
        }
    }
}