namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Convierte la entrada a mayusculas.
/// </summary>
public class UpperCasePipe : IPipe<string>
    {
    public string Transform(string input) => input.ToUpperInvariant();
    }
