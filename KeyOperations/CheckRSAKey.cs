using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA.KeyOperations;

public static class CheckRSAKey
{
    public static void Execute(
        string publicKeyFile = "rsa_public.pem",
        string privateKeyFile = "rsa_private.enc")
    {
        if (!File.Exists(publicKeyFile))
        {
            Loggers.LogError($"Public key file {publicKeyFile} not found.");
            throw new KeyException($"Public key file ({publicKeyFile}) not found.");
        }
        else if (new FileInfo(publicKeyFile).Length == 0)
        {
            Loggers.LogError($"Public key file {publicKeyFile} is empty.");
            throw new KeyException($"Public key file ({publicKeyFile}) is empty.");
        }

        if (!File.Exists(privateKeyFile))
        {
            Loggers.LogError($"Private key file {privateKeyFile} not found.");
            throw new KeyException($"Private key file ({privateKeyFile}) not found.");
        }
        else if (new FileInfo(privateKeyFile).Length == 0)
        {
            Loggers.LogError($"Private key file {privateKeyFile} is empty.");
            throw new KeyException($"Private key file ({privateKeyFile}) is empty.");
        }
    }
}