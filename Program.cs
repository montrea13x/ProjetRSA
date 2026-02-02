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
            { "encrypt", _ => ProjetRSA.MessageRSA.EncryptMessage() },
            { "encryptfile", _ => ProjetRSA.FileEncryptor.Encrypt() },
            { "decryptfile", _ => ProjetRSA.FileEncryptor.Decrypt() }
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
        dotnet run generate 2048       - Generate new RSA key pair, replacing 2048 with desired key size (2048, 3072 ou 4096)
        dotnet run read                - Read and display RSA keys
        dotnet run encrypt             - Encrypt a message using the public key
        dotnet run encryptfile         - Encrypt a file using the public key
        dotnet run decryptfile         - Decrypt a file using the private key
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
