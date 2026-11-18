using Becas1.CoreClassLibrary.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Convierte la entrada a "Tipo Título".
/// </summary>
public class NameCasePipe : IPipe<string> 
    {
    public string Transform(string input) => input.ToNameCase();
    }
