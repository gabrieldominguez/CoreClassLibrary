using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Escape;

/// <summary>
/// Escapa una cadena de C.
/// </summary>
public class CEscapePipe : IPipe<string> 
    {
    public string Transform(string input) => input.UnescapeCString();
    }
