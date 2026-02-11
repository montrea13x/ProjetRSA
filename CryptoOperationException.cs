namespace ProjetRSA;

public class CryptoOperationException : Exception
{
    public CryptoOperationException() { }

    public CryptoOperationException(string message) : base(message) { }

    public CryptoOperationException(string message, Exception innerException) : base(message, innerException) { }
}
