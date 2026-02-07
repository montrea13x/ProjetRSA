namespace ProjetRSA;

class Program
{
    static void Main(string[] args)
    {
        var commands = new Dictionary<string, Action<string[]>>(
            StringComparer.OrdinalIgnoreCase
            )
        {
            { "generate", ProjetRSA.KeyOperations.GenerateKeys.Execute },
            { "read", _ => ProjetRSA.KeyOperations.ReadKeys.Execute() },
            { "encryptfile", _ => ProjetRSA.FileEncryptor.Encrypt() },
            { "decryptfile", _ => ProjetRSA.FileEncryptor.Decrypt() },
            { "generaterootcert", _ => ProjetRSA.CertificateOperations.Certificate.GenerateRootCAFiles() },
            { "generateintermediatecert", _ => ProjetRSA.CertificateOperations.Certificate.GenerateIntermediateCAFiles() },
            { "generateendentitycert", _ => ProjetRSA.CertificateOperations.Certificate.GenerateEndEntityCertFiles() },
            { "signfile", _ => ProjetRSA.CertificateOperations.Signature.SignFile() },
            { "verifyfile", _ => ProjetRSA.CertificateOperations.Signature.VerifyFile() },
        };

        if (!ProjetRSA.CommandInput.CommandInput.TryExecute(args, commands))
        {
            ShowMenu();
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("""
        === RSA Key Management ===
        Usage:
        dotnet run generate 2048            - Generate new RSA key pair, replacing 2048 with desired key size (2048, 3072 ou 4096)
        dotnet run read                     - Read and display RSA keys
        dotnet run encryptfile              - Encrypt a file using the public key
        dotnet run decryptfile              - Decrypt a file using the private key
        dotnet run generaterootcert         - Generate a Root CA certificate + key
        dotnet run generateintermediatecert - Generate an Intermediate CA certificate + key
        dotnet run generateendentitycert    - Generate an End-Entity certificate + key
        dotnet run signfile                 - Sign a file (outputs Base64 signature file)
        dotnet run verifyfile               - Verify a file signature using the certificate
        """);
    }

    public static int ParseKeySize(string[] args)
    {
        if (args.Length == 0)
        {
            return 2048;
        }

        if (int.TryParse(args[0], out int size) && (size == 2048 || size == 3072 || size == 4096))
        {
            return size;
        }

        Console.WriteLine("Invalid key size. Using default 2048.");
        return 2048;
    }
}
