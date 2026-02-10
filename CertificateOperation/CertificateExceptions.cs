using System;

namespace ProjetRSA.CertificateOperations;

public class CertificateException : Exception
{
    public CertificateException() { }

    public CertificateException(string message) : base(message) { }

    public CertificateException(string message, Exception innerException) : base(message, innerException) { }
}