using CoreClassLibrary.Extensions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Becas1.CoreClassLibrary.RegularExpressions;

/// <summary>
/// Regular Expressions Extensions
/// </summary>
/// <see cref="https://devblogs.microsoft.com/dotnet/regular-expression-improvements-in-dotnet-7"/>
public static partial class RegexExtensions
    {
    [GeneratedRegex("\\w+", RegexOptions.CultureInvariant)]
    public static partial Regex WordsRegex();
    
    [GeneratedRegex("\\r\\n|\\n|\\r", RegexOptions.CultureInvariant)]
    public static partial Regex LineBreaks();
    
    [GeneratedRegex("\\\\[Uu]([0-9A-Fa-f]{4})")] 
    public static partial Regex UnicodeRegex();

    [GeneratedRegex("(.+?)", RegexOptions.CultureInvariant)]
    public static partial Regex EscapeCharsRegex();

    [GeneratedRegex("\\\\(.+?)", RegexOptions.CultureInvariant)]
    public static partial Regex UnescapeCharsRegex();

    [GeneratedRegex("([A-Z])", RegexOptions.CultureInvariant)]
    private static partial Regex UppercaseCharGroupRegex();

    [GeneratedRegex("\\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespacesRegex();

    [GeneratedRegex("_+", RegexOptions.CultureInvariant)]
    private static partial Regex UnderscoreRegex();

    [GeneratedRegex("[\\w|\\s]+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespacesOrWordsRegex();

    [GeneratedRegex("[a-zA-Z].*", RegexOptions.CultureInvariant)]
    private static partial Regex LettersRegex();

    [GeneratedRegex("\\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\\d|3[01])", RegexOptions.CultureInvariant)]
    public static partial Regex DateRegex();

    [GeneratedRegex("(20|23|24|27|30|33|34)\\d{9}", RegexOptions.CultureInvariant)]
    public static partial Regex CuitRegex();

    [GeneratedRegex("[/\\-\\s](?<key>\\w+)([:=]?((['\"](?<value>.*?)['\"])|(?<value>\\S+))?)", RegexOptions.CultureInvariant)]
    private static partial Regex CommandLineRegex();

    public static string CombinePatterns(this IEnumerable<string> patterns)
        => "(^".Concat(patterns.Interleave(")|(^")).Concat(")").Join();

    public static int GetSuccessfulGroupIndex(this Match match)
        =>  ((IEnumerable<Group>)match.Groups).Skip(1).Index().First(group => group.Item.Success).Index + 1;

    /// <summary>
    /// Unescape unicode string
    /// </summary>
    /// <example>
    /// "Cuenta cr\\u00E9dito no v\\u00E1lida\\u0000".UnescapeUnicodeString()   // "Cuenta crédito no válida\0"
    /// </example>
    /// <see cref="https://stackoverflow.com/questions/8558671/how-to-unescape-unicode-string-in-c-sharp"/>
    public static string UnescapeUnicodeString(this string input)
        => RegexExtensions.UnicodeRegex().Replace(input, m => char.ToString((char)ushort.Parse(m.Groups[1].Value, NumberStyles.AllowHexSpecifier)));

    /*
     * Instance of type 'System.Text.RegularExpressions.Regex.ValueSplitEnumerator' 
     * cannot be preserved across 'await' or 'yield' boundary.
     * 
    public static IEnumerable<ReadOnlyMemory<char>> GetLines(this ReadOnlyMemory<char> input)
        {
        foreach (Range range in RegexExtensions.LineBreaks().EnumerateSplits(input.Span))
            {
            yield return input[range];
            }
        }
    */

    /// <summary>
    /// Enumera todas las lineas de la entrada.
    /// </summary>
    [Obsolete("Use RegexExtensions.LineBreaks().EnumerateSplits or StringReader.ReadLine")]
    public static string[] GetLines(this string input)
        => RegexExtensions.LineBreaks().Split(input);

    /// <summary>
    /// Enumera todas las palabras de la entrada.
    /// </summary>
    /// <remarks>
    /// Esta función elimina los signos de puntuación,
    /// para conservarlos utilizar: Regex.Split(input, @"\s+");
    /// </remarks>
    public static IEnumerable<string> GetWords(this string input)
        => RegexExtensions.WordsRegex().Matches(input).Select(match => match.Value);

    /// <summary>
    /// Converts a string to its escaped representation.
    /// </summary>
    /// <param name="format"></param>
    /// <returns>Escaped representation</returns>
    /// <remarks>
    /// For use in DateTime.ToString()
    /// \c
    /// Where c is any character, the escape character displays the next character as a literal.
    /// The escape character cannot be used to create an escape sequence (like "\n" for new line)
    /// in this context.
    /// </remarks>
    public static string EscapeChars(this string format)
        => RegexExtensions.EscapeCharsRegex().Replace(format, @"\${1}");

    /// <summary>
    /// Converts a string to its unescaped representation.
    /// </summary>
    /// <param name="format"></param>
    /// <returns>Unescaped representation</returns>
    public static string UnescapeChars(this string format)
        => RegexExtensions.UnescapeCharsRegex().Replace(format, "${1}");


    /// <summary>
    /// Idem String.Replace pero ignorando minusculas/mayusculas en la busqueda.
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/6275980/string-replace-ignoring-case"/>
    /// <seealso cref="https://stackoverflow.com/questions/10078230/handling-regex-escape-replacement-text-that-contains-the-dollar-character/10078353#10078353"/>
    public static string ReplaceIgnoringCase(this string input, string search, string replacement)
        => Regex.Replace(input, Regex.Escape(search), replacement.Replace("$", "$$"), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Normaliza la cadena a "camelCase".
    /// </summary>
    /// <param name="camelSentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string FromCamelCase(this string camelSentence)
        => RegexExtensions.UppercaseCharGroupRegex().Replace(camelSentence, " ${1}");
    
    /// <summary>
    /// Normaliza la cadena "PascalCase".
    /// </summary>
    /// <param name="pascalSentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string FromPascalCase(this string pascalSentence)
        => RegexExtensions.FromCamelCase(pascalSentence).TrimStart();

    /// <summary>
    /// Normaliza la cadena "CONST_CASE".
    /// </summary>
    /// <param name="constSentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string FromConstCase(this string constSentence)
        => RegexExtensions.UnderscoreRegex().Replace(constSentence, " ").ToLowerInvariant();

    /// <summary>
    /// Convierte la cadena a "CONST_CASE".
    /// </summary>
    /// <param name="sentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string ToConstCase(this string sentence)
        => RegexExtensions.WhitespacesRegex().Replace(sentence, "_").ToUpperInvariant();

    /// <summary>
    /// Convierte la cadena a "camelCase".
    /// </summary>
    /// <param name="sentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string ToCamelCase(this string sentence)
        {
        if (sentence == "")
            {
            return "";
            }

        string pascalSentence = sentence.ToPascalCase();
        return string.Concat(char.ToLowerInvariant(pascalSentence[0]), pascalSentence[1..]);
        }

    /// <summary>
    /// Convierte la cadena a "PascalCase".
    /// </summary>
    /// <param name="sentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string ToPascalCase(this string sentence)
        => RegexExtensions.WhitespacesRegex().Replace(sentence.ToNameCase(), "");

    /// <summary>
    /// Convierte la cadena a "Tipo Título".
    /// </summary>
    /// <param name="sentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string ToNameCase(this string sentence)
        => RegexExtensions.WordsRegex().Replace(sentence, m => m.Value.CapitalizeFirstLetter());

    /// <summary>
    /// Convierte la cadena a "Tipo Título".
    /// </summary>
    /// <param name="sentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string ToTitleCase(this string sentence)
        => RegexExtensions.WhitespacesOrWordsRegex().Replace(sentence, m => m.ToString().ToTitleSentenceCase());

    /// <summary>
    /// Convierte la oracion a "Tipo Título".
    /// </summary>
    /// <param name="sentence">Cadena a convertir</param>
    /// <remarks>
    /// Si la cadena tiene tres letras o menos, es convertida a minusculas (no distingue abreviaciones).
    /// </remarks>
    /// <returns>Cadena convertida.</returns>
    public static string ToTitleSentenceCase(this string sentence)
        {
        bool isFirstWord = true;

        // Se divide la oracion en palabras.
        return RegexExtensions.WordsRegex().Replace(sentence, 
            m =>
                {
                string capWord = isFirstWord || m.Value.Length > 3 ? m.Value.CapitalizeFirstLetter() : m.Value.ToLowerInvariant();

                isFirstWord = false;
                return capWord;
                });
        }

    /// <summary>
    /// Convierte la cadena a "Tipo oración".
    /// </summary>
    /// <remarks>Si el primer caracter no es una letra es ignorado hasta encontrar la primer letra.</remarks>
    /// <param name="sentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string CapitalizeFirstLetter(this string sentence)
        => RegexExtensions.LettersRegex().Replace(sentence, m => m.Value.Capitalize());

    /// <summary>
    /// Separa las palabras de una linea para que queden equidistantes.
    /// </summary>
    public static string Justify(this string input, int width, char paddingChar = ' ')
        {
        if (input.Length >= width)
            {
            return input;
            }

        string[] words = RegexExtensions.WhitespacesRegex().Split(input);    // conserva los signos de puntuacion
        int last = words.Length - 1;
        int i = 0;
        while (words.Sum(word => word.Length) < width)
            {
            words[i++] += paddingChar;

            // recorro todas menos la ultima
            if (i >= last)
                {
                i = 0;
                }
            }

        return string.Concat(words);
        }

    /// <summary>
    /// Reemplaza palabras completas.
    /// </summary>
    public static string ReplaceWords(this string input, string search, string replacement = "", RegexOptions options = RegexOptions.None)
        => Regex.Replace(input, string.Concat("\\b", search, "\\b"), replacement, options | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    //=> Regex.Replace(sentence, @"\w+", m => m.Value == search ? replacement : m.Value, options);

    /// <summary>
    /// Reemplaza todas las subcadenas entre leftToken y rightToken.
    /// </summary>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="leftToken">Token izquierdo, si es vacio se toma desde el comienzo de la cadena.</param>
    /// <param name="rightToken">Token derecho, si es vacio se toma hasta el final de la cadena.</param>
    /// <returns>Cadenas obtenidas.</returns>
    public static string ReplaceAllTextBetweenTokens(this string input, string leftToken, string rightToken, string replacement = "", bool replaceTokens = false, RegexOptions options = RegexOptions.Singleline)
        => Regex.Replace(
            input,
            string.Concat(Regex.Escape(leftToken), ".*?", rightToken == "" ? ".*" : Regex.Escape(rightToken)),
            replaceTokens ? replacement : string.Concat(leftToken, replacement, rightToken),
            options | RegexOptions.Compiled | RegexOptions.CultureInvariant
            );

    /// <summary>
    /// Parsea la linea de comandos.
    /// </summary>
    /// <param name="commandLine">Cadena con la linea de comandos, puede incluir o no el nombre del ejecutable en primer lugar.</param>
    /// <remarks>
    /// Debe usarse / al comienzo del argumento.
    /// Debe usarse : o = como separador de argumento y valor (sin espacios).
    /// Se aceptan argumentos sin valor.
    /// Si el valor contiene espacios debe usarse ' o " como delimitador.
    /// Los formatos pueden combinarse en la misma linea.
    /// Puede incluir o no el nombre del ejecutable en primer lugar.
    ///
    /// No se admiten argumentos sin nombre o por default, por ej: dir *.*
    ///
    /// El nombre del argumento se usa como clave del diccionario.
    /// Si hay dos o más argumentos con el mismo nombre se produce una excepción.
    /// Se recomienda invocar siempre dentro de un bloque try{}catch.
    /// </remarks>
    /// <example>
    /// Formato de argumentos validos:
    /// "F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\" /install /a:all /b="all files" /t:'single quote' /h
    ///
    /// Formato de argumentos no validos:
    /// "F:\\Temp\\SampleWebApi.SelfHost.vshost.exe\" *.*
    ///
    /// <![CDATA[
    ///     Dictionary<string, string> dic = Environment.CommandLine.ParseCommandLine();
    /// ]]>
    /// </example>
    /// <returns>Diccionario con los argumentos y valores encontrados.</returns>
    public static Dictionary<string, string?> ParseCommandLine(this string commandLine)
        => RegexExtensions.CommandLineRegex()
            .Matches(commandLine)
            .ToDictionary(match => match.Groups["key"].Value.ToLowerInvariant(), match => match.Groups["value"].Success ? match.Groups["value"].Value : null);

    public static string ReplaceDate(this string input)
        => RegexExtensions.DateRegex().Replace(input, "{yyyy-MM-dd}");
    }
