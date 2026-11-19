using CoreClassLibrary.RegularExpressions;
using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Agrega espacios a la entrada y la convierte desde "PascalCase".
/// </summary>
public class FromPascalCasePipe : IPipe<string>
    {
    public string Transform(string input) => input.FromPascalCase();
    }
