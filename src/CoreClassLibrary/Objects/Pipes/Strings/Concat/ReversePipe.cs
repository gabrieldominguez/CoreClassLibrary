using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Concat;

/// <summary>
/// Invierte los caracteres de una cadena.
/// </summary>
public class ReversePipe : IPipe<string>
    {
    public string Transform(string input) 
        => input.Reverse().CreateString(input.Length);
    }
