using System;
using System.Collections.Generic;
using Xunit;
using System.Security.Cryptography;
using System.Text;


namespace unittests;

public class PrivateKeyEncryptorTests
{
    [Fact]
    public void EncryptPrivateKey()
    {
        string privateKeyPem = "FAKE PRIVATE KEY";
        string password = "StrongPassword123!";

        byte[] encrypted = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Encrypt(
            privateKeyPem,
            password
        );

        Assert.NotNull(encrypted);
        Assert.NotEmpty(encrypted);
    }

    [Fact]
    public void EncryptThenDecrypt_ReturnsOriginalText()
    {
        string originalText = "This is a secret private key";
        string password = "StrongPassword123!";

        byte[] encrypted = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Encrypt(
            originalText,
            password
        );

        string decrypted = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Decrypt(
            encrypted,
            password
        );

        Assert.Equal(originalText, decrypted);
    }

    [Fact]
    public void Decrypt_WithWrongPassword_ThrowsException()
    {
        string originalText = "This is a secret private key";
        string correctPassword = "CorrectPassword123!";
        string wrongPassword = "WrongPassword456!";

        byte[] encrypted = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Encrypt(
            originalText,
            correctPassword
        );

        Assert.Throws<CryptographicException>(() =>
        {
            ProjetRSA.KeyOperations.PrivateKeyEncryptor.Decrypt(
                encrypted,
                wrongPassword
            );
        });
    }

    [Fact]
    public void SaltDifferentEncryptedData()
    {
        string text = "This is a test string.";
        string password = "StrongPassword123!";

        byte[] encrypted1 = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Encrypt(
            text,
            password
        );

        byte[] encrypted2 = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Encrypt(
            text,
            password
        );

        Assert.NotNull(encrypted1);
        Assert.NotNull(encrypted2);

        Assert.NotEqual(encrypted1, encrypted2);
    }

    [Fact]
    public void Encrypt_EmptyString_ReturnsEncryptedData()
    {
        string emptyText = "";
        string password = "StrongPassword123!";

        byte[] encrypted = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Encrypt(
            emptyText,
            password
        );

        Assert.NotNull(encrypted);
        Assert.NotEmpty(encrypted);
    }

    [Fact]
public void EncryptAndDecrypt_EmptyPassword_WorksButIsInsecure()
{
    string text = "Some sensitive data";
    string emptyPassword = "";

    byte[] encrypted = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Encrypt(
        text,
        emptyPassword
    );

    string decrypted = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Decrypt(
        encrypted,
        emptyPassword
    );

    Assert.Equal(text, decrypted);
}

    [Fact]
    public void EncryptAndDecrypt_EmptyString_WorksCorrectly()
    {
        string emptyText = "";
        string password = "StrongPassword123!";

        byte[] encrypted = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Encrypt(
            emptyText,
            password
        );

        string decrypted = ProjetRSA.KeyOperations.PrivateKeyEncryptor.Decrypt(
            encrypted,
            password
        );

        Assert.Equal(emptyText, decrypted);
    }
}