using System.Text.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Escape;

/// <summary>
/// Convierte cualquier carácter de escape de las expresiones regulares en un carácter sin escape en la cadena de entrada.
/// </summary>
public class RegExUnescapePipe : IPipe<string> 
    {
    public string Transform(string input) => Regex.Unescape(input);
    }
