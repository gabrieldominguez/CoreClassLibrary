using System.Text.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Replace;

/// <summary>
/// Intercambia las subcadenas que están a la izquierda y derecha de una cadena especificada (separador).
/// </summary>
public class SwapPipe : IPipe<string>
    {
    private readonly ReplaceRegExPipe replaceRegExPipe;

    #region Properties
    private string separator = "";
    /// <summary>
    /// Cadena que funciona como separador de las subcadenas a intercambiar.
    /// </summary>
    public string Separator
        {
        get { return this.separator; }
        set
            {
            this.replaceRegExPipe.Pattern = String.Concat("(?<prefix>.*)", Regex.Escape(this.separator = value), "(?<sufix>.*)");
            this.replaceRegExPipe.Replacement = String.Concat("${sufix}", value, "${prefix}");
            }
        }
    #endregion 

    #region Constructors
    public SwapPipe() : this(new ReplaceRegExPipe(), "") { }

    public SwapPipe(ReplaceRegExPipe replaceRegExPipe, string separator) 
        {            
        (this.replaceRegExPipe = replaceRegExPipe).Options = RegexOptions.Multiline;
        this.Separator = separator;
        }
    #endregion 

    public string Transform(string input) => this.replaceRegExPipe.Transform(input);
    }
