using CoreClassLibrary.Extensions;
using System.Text.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Get;

/// <summary>
/// Recupera el texto entre los tokens.
/// </summary>
public class GetBetweenTokensPipe : IPipe<string>
    {
    private readonly GetRegExPipe getRegExPipe;

    /// <summary>
    /// Si es true, recupera todas las coincidencias y las devuelve separandolas con el separador especificado.
    /// </summary>
    public bool AllMatchesGet
        {
        get { return this.getRegExPipe.AllMatchesGet; }
        set { this.getRegExPipe.AllMatchesGet = value; }
        }

    /// <summary>
    /// Separador a utilizar cuando se obtienen todas las coincidencias.
    /// </summary>
    public string AllMatchesSeparator
        {
        get { return this.getRegExPipe.AllMatchesSeparator; }
        set { this.getRegExPipe.AllMatchesSeparator = value;}
        }

    /// <summary>
    /// Número de coincidencia a recuperar.
    /// </summary>
    /// <remarks>
    /// 0: 1er coincidencia
    /// n: n+1 coincidencia
    /// -n: total-n coincidencia
    /// </remarks>
    public int MatchIndex
        {
        get { return this.getRegExPipe.MatchIndex; }
        set { this.getRegExPipe.MatchIndex = value; }
        }

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
            }
        }
    /// <summary>
    /// Si es true, los tokens encontrados se incluiran en la salida.
    /// </summary>
    public bool GetTokens { get; set; } = false;

    public GetBetweenTokensPipe() : this(new GetRegExPipe()) { }

    public GetBetweenTokensPipe(GetRegExPipe getRegExPipe)
        {
        this.getRegExPipe = getRegExPipe;

        this.UpdatePattern();
        }

    private void UpdatePattern() => this.getRegExPipe.Pattern = String.Concat(escapedLeftToken, ".*?", escapedRightToken);

    public string Transform(string input)
        {
        string output = this.getRegExPipe.Transform(input);

        if(!this.GetTokens && output != "")
            {
            if(this.AllMatchesGet)
                {
                string[] lines = StringExtensions.Split(output, this.AllMatchesSeparator, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                    {
                    lines[i] = lines[i].Substring(this.leftToken.Length, lines[i].Length - this.leftToken.Length - this.rightToken.Length);
                    }

                return lines.Interleave(this.AllMatchesSeparator).Join();
                }

            return output.Substring(this.leftToken.Length, output.Length - this.leftToken.Length - this.rightToken.Length);
            }

        return output;
        }
    }
