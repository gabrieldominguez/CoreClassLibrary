using CoreClassLibrary.RegularExpressions;
using CoreClassLibrary.Objects.Pipes;

namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Elimina los espacios de la entrada y la convierte a "camelCase".
/// </summary>
public class CamelCasePipe : IPipe<string>
    {
    public string Transform(string input) => input.ToCamelCase();
    }
