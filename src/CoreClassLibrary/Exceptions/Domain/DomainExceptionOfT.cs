namespace CoreClassLibrary.Exceptions.Domain;

/// <summary>
/// Usar para excepciones de lógica del dominio.
/// </summary>
public class DomainException<T> : DomainException
    {
    /// <summary>
    /// Gets the object that contains the detail information of the validation condition.
    /// </summary>
    public T? Value { get; protected set; }

    public DomainException() { }

    public DomainException(string message) : base(message) { }
    public DomainException(Exception innerException) : base(innerException) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }

    public DomainException(string code, string message = "") : base(code, message) { }
    public DomainException(string code, string message, Exception innerException) : base(code, message, innerException) { }
    }
