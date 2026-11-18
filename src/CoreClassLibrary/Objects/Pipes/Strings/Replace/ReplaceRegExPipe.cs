using CoreClassLibrary.Objects.Pipes;
using System.Text.RegularExpressions;

namespace CoreClassLibrary.Objects.Pipes.Strings.Replace;

/// <summary>
/// Reemplaza todas las apariciones de un modelo de carácter definido por una expresión regular 
/// especificada por una cadena de caracteres de reemplazo, comenzando por el primer carácter. 
/// </summary>
public class ReplaceRegExPipe : IPipe<string>
    {
    /// <summary>
    /// Modelo de expresión regular del que van a buscarse coincidencias. 
    /// </summary>
    public string Pattern { get; set; } = "(.+)";

    /// <summary>
    /// Cadena de reemplazo.
    /// </summary>
    public string Replacement { get; set; } = "${1}";

    /// <summary>
    /// Combinación OR bit a bit de los valores de enumeración RegexOption.
    /// </summary>
    public RegexOptions Options { get; set; } = RegexOptions.Singleline;

    #region Constructors
    public ReplaceRegExPipe() { }

    public ReplaceRegExPipe(string pattern) : this(pattern, "${1}") { }

    public ReplaceRegExPipe(string pattern, string replacement) : this(pattern, replacement, RegexOptions.Singleline) { }

    public ReplaceRegExPipe(string pattern, string replacement, RegexOptions options)
        {
        this.Pattern = pattern;
        this.Replacement = replacement;
        this.Options = options;
        }
    #endregion 

    public string Transform(string input)
        => Regex.Replace(input, this.Pattern, this.Replacement, this.Options | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }
