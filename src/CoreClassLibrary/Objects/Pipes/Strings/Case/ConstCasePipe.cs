using CoreClassLibrary.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Elimina los espacios de la entrada y la convierte a "CONST_CASE".
/// </summary>
public class ConstCasePipe : IPipe<string>
    {
    public string Transform(string input) => input.ToConstCase();
    }
