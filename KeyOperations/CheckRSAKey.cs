using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA.KeyOperations;

public static class CheckRSAKey
{
    public static bool Execute(
        string publicKeyFile = "rsa_public.pem",
        string privateKeyFile = "rsa_private.enc")
    {
    bool valid = true;
        if (!File.Exists(publicKeyFile))
        {
            Console.WriteLine($"Public key file ({publicKeyFile}) not found.");
            valid = false;
        }
        else if (new FileInfo(publicKeyFile).Length == 0)
        {
            Console.WriteLine($"Public key file ({publicKeyFile}) is empty.");
            valid = false;
        }

        if (!File.Exists(privateKeyFile))
        {
            Console.WriteLine($"Private key file ({privateKeyFile}) not found.");
            valid = false;
        }
        else if (new FileInfo(privateKeyFile).Length == 0)
        {
            Console.WriteLine($"Private key file ({privateKeyFile}) is empty.");
            valid = false;
        }
        return valid;
    }
}