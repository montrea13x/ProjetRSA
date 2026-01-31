using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA.FileOperations;

public static class EncryptFile
{
    public static void Execute()
    {
        const string publicKeyFile = "rsa_public.pem";
        const string inputFile = "test.txt";
        const string outputFile = "test.enc";

        if (!File.Exists(publicKeyFile))
        {
            Console.WriteLine("Public key file rsa_public.pem not found.");
            Console.WriteLine("Do you want to generate a new key pair (2048)? (y/n): ");
            string? response = Console.ReadLine();
            if (response?.ToLower() == "y" || response?.ToLower() == "yes")
            {
                ProjetRSA.KeyOperations.GenerateKeys.Execute(new string[] { "2048" });
            }
            else
            {
                Console.WriteLine("Cannot proceed without public key.");
                return;
            }
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"File to encrypt ({inputFile}) not found.");
            return;
        }

        byte[] FileToEncrypt = File.ReadAllBytes(inputFile);

        string publicKeyPem = File.ReadAllText(publicKeyFile);
        using RSA rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);

        // Generate AES key and IV
        byte[] aesKey = RandomNumberGenerator.GetBytes(32); // AES-256
        byte[] iv = RandomNumberGenerator.GetBytes(16);     // Bloc AES

        // Chiffrer le fichier avec AES
        byte[] encryptedFile;
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = aesKey;
            aes.IV = iv;

            using var ms = new MemoryStream();
            using var encryptor = aes.CreateEncryptor();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

            cs.Write(FileToEncrypt, 0, FileToEncrypt.Length);
            cs.FlushFinalBlock();

            encryptedFile = ms.ToArray();
        }

        // Chiffrer la clé AES avec RSA
        byte[] encryptedAesKey = rsa.Encrypt(
            aesKey,
            RSAEncryptionPadding.OaepSHA256
        );

        //Écrire le fichier chiffré final
        using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
        using (var bw = new BinaryWriter(fs))
        {
            // Écrire la longueur de la clé AES chiffrée
            bw.Write(encryptedAesKey.Length);
            // Écrire la clé AES chiffrée
            bw.Write(encryptedAesKey);
            // Écrire l'IV
            bw.Write(iv);
            // Écrire le fichier chiffré
            bw.Write(encryptedFile);
        }
        Console.WriteLine($"File encrypted Successfully.");
        Console.WriteLine($"Output file: {outputFile}");
    }
}
