using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA.KeyOperations;

public static class CheckRSAKey
{
    public static bool Execute()
    {
    bool valid = true;
        if (!File.Exists("rsa_public.pem"))
        {
            Console.WriteLine("Public key file rsa_public.pem not found.");
            valid = false;
        }
        else if (new FileInfo("rsa_public.pem").Length == 0)
        {
            Console.WriteLine("Public key file rsa_public.pem is empty.");
            valid = false;
        }

        if (!File.Exists("rsa_private.enc"))
        {
            Console.WriteLine("Private key file rsa_private.enc not found.");
            valid = false;
        }
        else if (new FileInfo("rsa_private.enc").Length == 0)
        {
            Console.WriteLine("Private key file rsa_private.enc is empty.");
            valid = false;
        }
        return valid;
    }
}