using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA.KeyOperations;

public static class DecryptPrivateKey
{
    public static string Execute(byte[] encryptedData, string password)
    {
        // Extraire les données de salt et de texte chiffré
        byte[] salt = encryptedData[..16];
        byte[] ciphertext = encryptedData[16..];

        // Dériver la même clé en utilisant le salt extrait
        byte[] derived = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            150000,
            HashAlgorithmName.SHA256,
            48
        );

        byte[] key = derived[..32]; // Clé 256-bits
        byte[] iv = derived[32..48];  // IV 128-bits

        // Créer un chiffrement AES avec la clé et l'IV dérivés
        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key;
        aes.IV = iv;
        
        // Créer un décrypteur
        using var ms = new MemoryStream(ciphertext);
        using var decryptor = aes.CreateDecryptor();
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);
        
        return sr.ReadToEnd();
        
    }
}