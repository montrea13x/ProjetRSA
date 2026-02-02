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
                Console.WriteLine("\nPasswords do not match. Please try again.");
                
                Console.Write("Enter password to encrypt private key: ");
                password = PasswordHelper.ReadPassword();
                
                Console.Write("\nConfirm password: ");
                confirmPassword = PasswordHelper.ReadPassword();
            }

            byte[] encryptedPrivateKey = PrivateKeyEncryptor.Encrypt(privateKeyPem, password);
            
            password = string.Empty;
            confirmPassword = string.Empty;

            File.WriteAllBytes("rsa_private.enc", encryptedPrivateKey);
            File.WriteAllText("rsa_public.pem", publicKeyPem, Encoding.ASCII);

            Console.WriteLine($"\nRSA key pair generated ({keySize} bits).");
            Console.WriteLine("Saved rsa_private.enc (encrypted) and rsa_public.pem.");

        }
        catch (CryptographicException ex)
        {
            Console.WriteLine($"Cryptographic error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}