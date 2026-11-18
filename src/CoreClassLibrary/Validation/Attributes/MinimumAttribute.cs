
using System.ComponentModel.DataAnnotations;
using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Validation.Attributes;

/// <summary>
/// Specifies the minimum value of a data field.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class MinimumAttribute : RangeAttribute
    {
    public MinimumAttribute(int minimum): base(minimum, int.MaxValue) { }
    public MinimumAttribute(double minimum) : base(minimum, double.MaxValue) { }
    public MinimumAttribute(DateTime minimum) : base(typeof(DateTime), minimum.ToLocalDateTimeString(), DateTime.MaxValue.ToLocalDateTimeString()) { }

    public override string FormatErrorMessage(string name)
        => $"The {name} field must be greater than {this.Minimum}.";
    }
