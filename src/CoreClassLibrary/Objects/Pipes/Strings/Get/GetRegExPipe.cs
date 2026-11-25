using CoreClassLibrary.Objects.Pooling;
using System.Text.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Get;

/// <summary>
/// Recupera una subcadena de la entrada, utilizando espresiones regulares.
/// </summary>
public class GetRegExPipe : IPipe<string>
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
    public GetRegExPipe() { }

    public GetRegExPipe(string pattern) : this(pattern, RegexOptions.None) { }

    public GetRegExPipe(string pattern, RegexOptions options):this(pattern, options, 0) { }

    public GetRegExPipe(string pattern, int matchIndex):this(pattern, RegexOptions.None, matchIndex) { }

    public GetRegExPipe(string pattern, RegexOptions options, int matchIndex)
        {
        this.Pattern = pattern;
        this.Options = options;
        this.MatchIndex = matchIndex;
        }
    #endregion 

    public string Transform(string input)
        {
        if(this.AllMatchesGet || this.MatchIndex != 0)
            {
            MatchCollection matchs = Regex.Matches(input, this.Pattern, this.Options | RegexOptions.Compiled | RegexOptions.CultureInvariant);

            if(this.AllMatchesGet)
                {
                if (matchs.Count > 0)
                    {
                    using PooledStringBuilder builder = new PooledStringBuilder();
                    foreach(Match match in matchs.Cast<Match>())
                        {
                        builder.StringBuilder.Append(match.Value).Append(this.AllMatchesSeparator);
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
                return -matchs.Count <= this.MatchIndex && this.MatchIndex < matchs.Count
                    ? matchs[this.MatchIndex < 0? matchs.Count + this.MatchIndex: this.MatchIndex].Value
                    : "";
                }
            }
        else
            {
            return Regex.Match(input, this.Pattern, this.Options | RegexOptions.Compiled | RegexOptions.CultureInvariant).Value;
            }
        }
    }
