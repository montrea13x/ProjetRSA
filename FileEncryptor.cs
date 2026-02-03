using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA;


class FileEncryptor
{
    public static void Encrypt(
        string publicKeyFile = "rsa_public.pem",
        string inputFile = "test.txt",
        string outputFile = "test.enc"
        )
    {

        if (!File.Exists(publicKeyFile))
        {
            Console.WriteLine("Public key file rsa_public.pem not found.");
            return;
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

    public static void Decrypt(
        string privateKeyFile = "rsa_private.enc",
        string inputFile = "test.enc",
        string outputFile = "test_decrypted.txt"
        )
    {
        

        if (!File.Exists(privateKeyFile))
        {
            Console.WriteLine("Private key file rsa_private.enc not found.");
            return;
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"File to decrypt ({inputFile}) not found.");
            return;
        }

        byte[] encryptedFileContent = File.ReadAllBytes(inputFile);

        using RSA? rsa = ProjetRSA.KeyOperations.KeyLoader.TryLoadPrivateKey(privateKeyFile);
        if (rsa == null)
        {
            return;
        }

        using var ms = new MemoryStream(encryptedFileContent);
        using var br = new BinaryReader(ms);

        // Lire la longueur de la clé AES chiffrée
        int encryptedAesKeyLength = br.ReadInt32();
        // Lire la clé AES chiffrée
        byte[] encryptedAesKey = br.ReadBytes(encryptedAesKeyLength);
        // Lire l'IV
        byte[] iv = br.ReadBytes(16);
        // Lire le fichier chiffré
        byte[] encryptedFile = br.ReadBytes((int)(ms.Length - ms.Position));

        // Déchiffrer la clé AES avec RSA
        byte[] aesKey = rsa.Decrypt(
            encryptedAesKey,
            RSAEncryptionPadding.OaepSHA256
        );

        // Déchiffrer le fichier avec AES
        byte[] decryptedFile;
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = aesKey;
            aes.IV = iv;

            using var msDecrypt = new MemoryStream();
            using var decryptor = aes.CreateDecryptor();
            using var cs = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write);

            cs.Write(encryptedFile, 0, encryptedFile.Length);
            cs.FlushFinalBlock();

            decryptedFile = msDecrypt.ToArray();
        }

        File.WriteAllBytes(outputFile, decryptedFile);

        Console.WriteLine("File decrypted successfully.");
        Console.WriteLine($"Output file: {outputFile}");
    }
}