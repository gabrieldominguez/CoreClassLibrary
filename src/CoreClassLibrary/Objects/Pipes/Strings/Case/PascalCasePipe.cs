using CoreClassLibrary.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Elimina los espacios de la entrada y la convierte a "PascalCase".
/// </summary>
public class PascalCasePipe : IPipe<string>
    {
    public string Transform(string input) => input.CapitalizeFirstLetter();
    }
