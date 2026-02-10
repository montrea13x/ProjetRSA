namespace ProjetRSA;

public class Exceptions : Exception
{
    public Exceptions() { }

    public Exceptions(string message) : base(message) { }

    public Exceptions(string message, Exception innerException) : base(message, innerException) { }
}