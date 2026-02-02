using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA;

class MessageRSA
{
    public static void EncryptMessage(
        string publicKeyFile = "rsa_public.pem"
    )
    {
        if(File.Exists(publicKeyFile) == false)
        {
            Console.WriteLine($"Public key file {publicKeyFile} not found.");
            Console.WriteLine("Do you want to generate a new key pair? (y/n): ");
            string? response = Console.ReadLine();
            if (response != null && response.ToLower() == "y" || response != null && response.ToLower() == "yes")
            {
                ProjetRSA.KeyOperations.GenerateKeys.Execute(new string[] { "2048" });
            }
            else
            {
                Console.WriteLine("Cannot proceed without public key.");
                return;
            }
        }



        Console.WriteLine("=== Generate Message ===\n");
        Console.WriteLine("Enter message to encrypt: ");

        string? message = Console.ReadLine();
        while (string.IsNullOrEmpty(message))
        {
            Console.WriteLine("No message entered. Please enter a valid message: ");
            message = Console.ReadLine();
        }
        string publicKeyPem = File.ReadAllText(publicKeyFile);
    }
}