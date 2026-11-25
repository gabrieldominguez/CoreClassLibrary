using CoreClassLibrary.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Case;

/// <summary>
/// Convierte la entrada a "Tipo Título".
/// </summary>
public class TitleCasePipe : IPipe<string> 
    {
    public string Transform(string input) => input.ToTitleCase();
    }
