using CoreClassLibrary.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Convierte la entrada a "Tipo oración".
/// </summary>
public class CapitalizePipe : IPipe<string> 
    {
    public string Transform(string input) => input.CapitalizeFirstLetter();
    }
