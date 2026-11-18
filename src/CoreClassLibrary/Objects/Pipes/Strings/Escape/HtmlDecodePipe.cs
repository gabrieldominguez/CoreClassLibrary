using System.Web;

namespace CoreClassLibrary.Objects.Pipes.Strings.Escape;

/// <summary>
/// Decodifica la entrada HTML
/// </summary>
public class HtmlDecodePipe : IPipe<string>
    {
    public string Transform(string input) => HttpUtility.HtmlDecode(input);
    }
