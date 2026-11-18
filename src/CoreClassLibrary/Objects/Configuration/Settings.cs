namespace CoreClassLibrary.Objects.Configuration;

/// <summary>
/// Represents a setting for a value, encapsulating it.
/// </summary>
/// <remarks>
/// This record CANNOT be used with the Options pattern due to the requirement for a parameterless constructor.
/// So it is intended to be used with the GetRequiredSetting or TryGetSetting extension methods, which can handle records with parameterized constructors. 
/// Its value cannot be updated if the configuration is changed, 
/// so it is intended to be used for setting that should not change during the application's lifetime, 
/// even if the underlying configuration source is modified.
/// If the underlying configuration source is modified, a restart will be required to apply the new value, as the setting will not automatically update at runtime.
/// </remarks>
public record Settings<T>(T Value);
