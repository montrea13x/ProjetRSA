using System;
using System.IO;
using System.Security.Cryptography;

namespace ProjetRSA.KeyOperations;

class KeyLoader
{
    public static RSA? TryLoadPrivateKey(string privateKeyFile)
    {
        if (!File.Exists(privateKeyFile))
        {
            Console.WriteLine($"Private key file {privateKeyFile} not found.");
            return null;
        }

        byte[] encryptedPrivateKey = File.ReadAllBytes(privateKeyFile);

        Console.Write("Enter password to decrypt private key: ");
        string password = PasswordHelper.ReadPassword();

        string privateKeyPem;
        try
        {
            privateKeyPem = PrivateKeyEncryptor.Decrypt(encryptedPrivateKey, password);
        }
        catch (CryptographicException)
        {
            Console.WriteLine("Invalid password or corrupted private key.");
            return null;
        }

        RSA rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);
        return rsa;
    }
}
