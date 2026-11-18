using CoreClassLibrary.Extensions;
using System.ComponentModel.DataAnnotations;

namespace CoreClassLibrary.Exceptions.Domain;

/// <summary>
/// Usar para excepciones de lógica del dominio.
/// </summary>
/// <remarks>
/// ValidationException can carry only one ValidationResult, so I use DomainException instead.
/// </remarks>
/// <see cref="Validation.ValidationExtensions"/>
public class DomainException : Exception
    {
    public string Code { get; } = "";
    public ICollection<ValidationResult> ValidationResults { get; } = [];

    public DomainException() { }

    public DomainException(IDictionary<string, string[]> errors): this(DomainException.GetValidationResults(errors)) { }
    public DomainException(ICollection<ValidationResult> validationResults): this("One or more validation errors occurred.") { ValidationResults = validationResults; }

    public DomainException(DomainExceptionMessageDetail domainException) : this(domainException.Code, domainException.Message) { }
    public DomainException(DomainExceptionDetail domainException) : this(domainException.Code, domainException.Message) { }

    public DomainException(string message) : base(message) { }
    public DomainException(string code, string message) : base(message) { this.Code = code; }

    public DomainException(Exception innerException) : this(innerException.GetType().Name, innerException.Message, innerException) { }
    public DomainException(string code, string message, Exception innerException) : base(message, innerException) { this.Code = code; }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }

    public IDictionary<string, string[]> GetValidationErrors()
        => DomainException.GetValidationErrors(this.ValidationResults);

    public static IDictionary<string, string[]> GetValidationErrors(IEnumerable<ValidationResult> validationResults)
        => validationResults
            .GroupBy(
                result => result.MemberNames.FirstOrDefault() ?? Guid.NewGuid().ToString(),
                result => result.ErrorMessage ?? "" 
                ).ToArrayDictionary();

    public static ICollection<ValidationResult> GetValidationResults(IDictionary<string, string[]> errors)
        => errors
            .SelectMany(error => error.Value.Select(error.Key, (value, key) => (key, value)))
            .Select(error => new ValidationResult(error.value, [ error.key ]))
            .ToList();            
    }