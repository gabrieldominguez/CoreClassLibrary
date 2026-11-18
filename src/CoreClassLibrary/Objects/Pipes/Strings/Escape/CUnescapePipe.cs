using CoreClassLibrary.Extensions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Escape;

/// <summary>
/// DeEscapa una cadena de C.
/// </summary>
public class CUnescapePipe : IPipe<string> 
    {
    public string Transform(string input) => input.EscapeCString();
    }
