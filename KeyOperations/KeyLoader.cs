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
            Loggers.LogError($"Private key file {privateKeyFile} not found.");
            throw new KeyException($"Private key file {privateKeyFile} not found.");
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
            Loggers.LogError("Invalid password or corrupted private key.");
            throw new KeyException("Invalid password or corrupted private key.");
        }

        RSA rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);
        return rsa;
    }
}
