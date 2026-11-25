using System.Text.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Replace;

/// <summary>
/// Reemplaza el texto entre los tokens, por una cadena especificada.
/// </summary>
public class ReplaceBetweenTokensPipe : IPipe<string>
    {
    private readonly ReplaceRegExPipe replaceRegExPipe;

    #region Properties
    private string escapedLeftToken = "";
    private string leftToken = "";
    /// <summary>
    /// Token izquierdo.
    /// </summary>
    public string LeftToken
        {
        get { return this.leftToken; }
        set
            {
            escapedLeftToken = Regex.Escape(this.leftToken = value);

            this.UpdatePattern();
            this.UpdateReplace();
            }
        }

    private string escapedRightToken = "";
    private string rightToken = "";
    /// <summary>
    /// Token derecho.
    /// </summary>
    public string RightToken
        {
        get { return this.rightToken; }
        set
            {
            escapedRightToken = (this.rightToken = value) == "" ? ".*" : Regex.Escape(this.rightToken = value);

            this.UpdatePattern();
            this.UpdateReplace();
            }
        }

    private bool replaceTokens = false;
    /// <summary>
    /// Si es true, los tokens encontrados se reemplazarán.
    /// </summary>
    public bool ReplaceTokens
        {
        get { return this.replaceTokens; }
        set
            {
            this.replaceTokens = value;

            this.UpdateReplace();
            }
        }

    private string replacement = "";
    /// <summary>
    /// Cadena de reemplazo.
    /// </summary>
    public string Replacement
        {
        get { return this.replacement; }
        set
            {
            this.replacement = value;

            this.UpdateReplace();
            }
        }

    private void UpdatePattern() => this.replaceRegExPipe.Pattern = String.Concat(escapedLeftToken, ".*?", escapedRightToken);

    private void UpdateReplace() => this.replaceRegExPipe.Replacement = this.replaceTokens ? this.replacement : String.Concat(this.leftToken, this.replacement, this.rightToken);
    #endregion 

    #region Constructors
    public ReplaceBetweenTokensPipe() : this(new ReplaceRegExPipe()) { }

    public ReplaceBetweenTokensPipe(ReplaceRegExPipe replaceRegExPipe) 
        {
        this.replaceRegExPipe = replaceRegExPipe;
        this.UpdateReplace();
        this.UpdatePattern();
        }
    #endregion 

    public string Transform(string input) => this.replaceRegExPipe.Transform(input);
    }
