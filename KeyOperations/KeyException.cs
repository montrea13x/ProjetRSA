namespace ProjetRSA.KeyOperations;

public class KeyException : Exception
{
    public KeyException() { }

    public KeyException(string message) : base(message) { }

    public KeyException(string message, Exception innerException) : base(message, innerException) { }
}