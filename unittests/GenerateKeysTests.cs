using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Xunit;

namespace unittests;

public class GenerateKeysTests
{
    [Fact]
    public void GenerateRSAKeyPair()
    {
        using var rsa = RSA.Create(2048);

        byte[] privateKey = rsa.ExportPkcs8PrivateKey();
        byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();

        Assert.NotNull(privateKey);
        Assert.NotNull(publicKey);
        Assert.True(privateKey.Length > 0);
        Assert.True(publicKey.Length > 0);
    }

    [Fact]
    public void PemEncoding_ValidFormat()
    {
        using var rsa = RSA.Create(2048);

        string pem = new string(
            PemEncoding.Write(
                "PRIVATE KEY",
                rsa.ExportPkcs8PrivateKey()
            )
        );

        Assert.Contains("BEGIN PRIVATE KEY", pem);
        Assert.Contains("END PRIVATE KEY", pem);
    }

    [Fact]
    public void PublicKey_ShouldMatchPrivateKey()
    {
        using var rsa = RSA.Create(2048);

        byte[] privateKey = rsa.ExportPkcs8PrivateKey();
        byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();

        using var rsaFromPrivate = RSA.Create();
        rsaFromPrivate.ImportPkcs8PrivateKey(privateKey, out _);

        byte[] regeneratedPublicKey =
            rsaFromPrivate.ExportSubjectPublicKeyInfo();

        Assert.Equal(publicKey, regeneratedPublicKey);
    }
}