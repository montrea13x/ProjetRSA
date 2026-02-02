using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA.FileOperations;

public static class DecryptFile
{
public static void Execute(
    string inputFile = "test.enc", 
    string outputFile = "test_decrypted.txt",
    string privateKeyFile = "rsa_private.enc")
    {
        if (!File.Exists(privateKeyFile))
        {
            Console.WriteLine($"Private key file ({privateKeyFile}) not found.");
            return;
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"File to decrypt ({inputFile}) not found.");
            return;
        }

        byte[] encryptedFileContent = File.ReadAllBytes(inputFile);

        // Lire la clé privée chiffrée
        byte[] encryptedPrivateKey = File.ReadAllBytes(privateKeyFile);

        Console.Write("Enter password to decrypt private key: ");
        string password = PasswordHelper.ReadPassword();

        string privateKeyPem;
        try
        {
            privateKeyPem = ProjetRSA.KeyOperations.DecryptPrivateKey.Execute(encryptedPrivateKey, password);
        }
        catch (CryptographicException)
        {
            Console.WriteLine("Invalid password or corrupted private key.");
            return;
        }

        using RSA rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);

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
