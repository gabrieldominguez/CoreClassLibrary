using CoreClassLibrary.Exceptions.Domain;
using System.ComponentModel.DataAnnotations;

namespace CoreClassLibrary.Validation;

/// <summary>
/// Validator extensions
/// </summary>
/// <remarks>
/// ValidateObject() fails fast by throwing an exception with the first validation error. If you want to know ALL of the validation errors, use TryValidateObject() instead.
/// When validateAllParameters is false (which is the default), it only validates properties that have the [Required] attribute.
/// 
/// ValidationException can carry only one ValidationResult, so I use DomainException instead.
/// </remarks>
/// <see cref="DomainException"/>
/// <see cref="https://stackoverflow.com/questions/5636975/validator-validateobject-with-validateallproperties-to-true-stop-at-first-erro"/>
/// <see cref="https://makolyte.com/csharp-how-to-manually-run-validation-on-objects-that-have-model-validation-attributes/"/>
public static class ValidationExtensions
    {
    public static void Validate(this object instance)
        => instance.TryValidate().ThrowIfAny();

    public static ICollection<ValidationResult> TryValidate(this object instance)
        {
        ICollection<ValidationResult> validationResults = [];

        Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

        return validationResults;
        }

    public static void ThrowIfAny(this ICollection<ValidationResult> validationResults)
        {
        if (validationResults.Count > 0)
            {
            throw new DomainException(validationResults);
            }
        }

    public static void ThrowIfNotSuccess(this ValidationResult? validationResult)
        {
        if (validationResult != null && validationResult != ValidationResult.Success)
            {
            throw new DomainException([ validationResult ]);
            }
        }
    }
