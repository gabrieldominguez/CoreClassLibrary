using System.ComponentModel.DataAnnotations;

namespace CoreClassLibrary.Validation.Attributes;

/// <summary>
/// Specifies the length of characters that are allowed in a string data field.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class FixedLengthStringAttribute : StringLengthAttribute
    {
    public FixedLengthStringAttribute(int length): base(length)
        {
        this.MinimumLength = length;
        }

    public override string FormatErrorMessage(string name)
        => $"The field {name} must be a string with a length of {this.MinimumLength}.";
    }
