using System.ComponentModel.DataAnnotations;

namespace CoreClassLibrary.Objects.Configuration;

/// <summary>
/// Represents a options for a value, encapsulating it.
/// </summary>
/// <remarks>
/// This record can be used with the Options pattern due to the requirement for a parameterless constructor.
/// So it is intended to be used with the AddOption extension methods.
/// Its value will be updated if the configuration is changed if it is registered as a monitored or scoped options.
/// </remarks>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/core/extensions/options"/>
/// <see cref="https://andrewlock.net/using-multiple-instances-of-strongly-typed-settings-with-named-options-in-net-core-2-x/"/>
/// <see cref="https://andrewlock.net/adding-validation-to-strongly-typed-configuration-objects-in-asp-net-core/"/>
public record Options<T>
    {
    [Required]
    public required T Value { get; init; }
    }
