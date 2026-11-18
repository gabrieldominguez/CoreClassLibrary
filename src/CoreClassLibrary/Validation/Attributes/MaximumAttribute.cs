
using System.ComponentModel.DataAnnotations;
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Validation.Attributes;

/// <summary>
/// Specifies the maximum value of a data field.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class MaximumAttribute : RangeAttribute
    {
    public MaximumAttribute(int maximum): base(int.MinValue, maximum) { }
    public MaximumAttribute(double maximum) : base(double.MinValue, maximum) { }
    public MaximumAttribute(DateTime maximum) : base(typeof(DateTime), DateTime.MinValue.ToLocalDateTimeString(), maximum.ToLocalDateTimeString()) { }

    public override string FormatErrorMessage(string name)
        => $"The {name} field must be lower than {this.Maximum}.";
    }
