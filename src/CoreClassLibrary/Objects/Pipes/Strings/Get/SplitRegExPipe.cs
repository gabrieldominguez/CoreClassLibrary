using CoreClassLibrary.Objects.Pooling;
using System.Text.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Get;

/// <summary>
/// Identifica las subcadenas de la entrada que están delimitadas por una expresión regular especificada, 
/// y recupera la especificada.
/// </summary>
public class SplitRegExPipe : IPipe<string>
    {
    /// <summary>
    /// Modelo de expresión regular del que van a buscarse coincidencias.
    /// </summary>
    public string Pattern { get; set; } = ".*";

    /// <summary>
    /// Combinación OR bit a bit de los valores de enumeración RegexOption.
    /// </summary>
    public RegexOptions Options { get; set; } = RegexOptions.None;

    /// <summary>
    /// Si es true, recupera todas las coincidencias y las devuelve separandolas con el separador especificado.
    /// </summary>
    public bool AllMatchesGet { get; set; } = false;

    /// <summary>
    /// Separador a utilizar cuando se obtienen todas las coincidencias.
    /// </summary>
    public string AllMatchesSeparator { get; set; } = "\r\n";

    /// <summary>
    /// Número de coincidencia a recuperar.
    /// </summary>
    /// <remarks>
    /// 0: 1er coincidencia
    /// n: n+1 coincidencia
    /// -n: total-n coincidencia
    /// </remarks>
    public int MatchIndex { get; set; } = 0;

    #region Constructors
    public SplitRegExPipe() { }

    public SplitRegExPipe(string pattern) : this(pattern, RegexOptions.None) { }

    public SplitRegExPipe(string pattern, RegexOptions options) : this(pattern, options, 0) { }

    public SplitRegExPipe(string pattern, int matchIndex) : this(pattern, RegexOptions.None, matchIndex) { }

    public SplitRegExPipe(string pattern, RegexOptions options, int matchIndex)
        {
        this.Pattern = pattern;
        this.Options = options;
        this.MatchIndex = matchIndex;
        }
    #endregion

    public string Transform(string input)
        {
        string[] splits = Regex.Split(input, this.Pattern, this.Options | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        if(this.AllMatchesGet)
            {
            if (splits.Length > 0)
                {
                using PooledStringBuilder builder = new PooledStringBuilder();
                foreach(String split in splits)
                    {
                    builder.StringBuilder.Append(split).Append(this.AllMatchesSeparator);
                    }
                return builder.StringBuilder.ToString(0, builder.StringBuilder.Length - this.AllMatchesSeparator.Length);
                }
            else
                {
                return "";
                }
            }
        else
            {
            return -splits.Length <= this.MatchIndex && this.MatchIndex < splits.Length
                ? splits[this.MatchIndex < 0 ? splits.Length + this.MatchIndex: this.MatchIndex]
                : "";
            }
        }
    }
