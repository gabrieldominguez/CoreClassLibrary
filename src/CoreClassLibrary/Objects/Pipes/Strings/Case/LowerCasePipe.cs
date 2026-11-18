using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Convierte la entrada a minusculas.
/// </summary>
public class LowerCasePipe : IPipe<string>
    {
    public string Transform(string input) => input.ToLowerInvariant();
    }
