using System.ComponentModel.DataAnnotations;

namespace CoreClassLibrary.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class EqualsAttribute(object validValue) : ValidationAttribute
    {
    public override bool IsValid(object? value)
        => validValue.Equals(value);

    public override string FormatErrorMessage(string name)
        => $"The {name} field must be equal to {validValue}.";
    }
