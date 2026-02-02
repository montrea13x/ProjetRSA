using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProjetRSA.KeyOperations;

class PrivateKeyEncryptor
{
    public static byte[] Encrypt(
        string plainText,
        string password
        )
    {
        // Générer un salt aléatoire de 16 octets pour la dérivation de clé
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        
        // Dériver 48 octets (32 pour la clé AES-256 + 16 pour l'IV) avec PBKDF2 et SHA-256
        // 10000 itérations pour une dérivation sécurisée
        byte[] derived = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            150000,
            HashAlgorithmName.SHA256,
            48
        );

        // Diviser les octets dérivés: les 32 premiers pour la clé AES, les 16 derniers pour le vecteur d'initialisation
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

        // Créer un crypteur
        using var ms = new MemoryStream();

        // Écrire le salt au début pour pouvoir dériver la même clé lors du déchiffrement
        ms.Write(salt, 0, salt.Length);

        using (var encryptor = aes.CreateEncryptor())
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plainText);
        }

        return ms.ToArray();
    }

    public static string Decrypt(
        byte[] encryptedData,
        string password
        )
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