using CoreClassLibrary.Objects.Pipes;
using System.Web;

namespace CoreClassLibrary.Objects.Pipes.Strings.Escape;

/// <summary>
/// Codifica la entrada a HTML
/// </summary>
public class HtmlEncodePipe : IPipe<string>
    {
    public string Transform(string input) => HttpUtility.HtmlEncode(input);
    }
