#pragma warning disable IDE0305 // Simplify collection initialization
#pragma warning disable IDE0046 // Convert to conditional expression
#pragma warning disable IDE0057 // Use range operator

using Becas1.CoreClassLibrary.RegularExpressions;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Buffers;
using System.Globalization;
using System.Text;
using System.Web;

// Para remover las regiones
// Reemplazar por vacio usando expresiones regulares: ^[ \t]*\#[ \t]*(region|endregion).*\n

namespace CoreClassLibrary.Extensions;

public enum Aligning
    {
    None,
    Left,
    Right,
    Center,
    Justify
    }

/// <summary>
/// String extensions
/// </summary>
/// <see cref="https://github.com/Humanizr/Humanizer"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding-introduction"/>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types#utf-8-string-literals"/>
public static class StringExtensions
    {
    public static readonly string UpperCaseAlphaChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static readonly string LowerCaseAlphaChars = "abcdefghijklmnopqrstuvwxyz";
    public static readonly string AlphaChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    public static readonly string AlphaCharsWithDiacritics = "ABCDEFGHIJKLMNOPQRSTUVWXYZÁÉÍÓÚÀÈÌÒÙÄËÏÖÜÂÊÎÔÛÑabcdefghijklmnopqrstuvwxyzáéíóúàèìòùäëïöüâêîôûñ";
    public static readonly string NumericChars = "0123456789";
    public static readonly string HEXChars = "0123456789ABCDEF";
    public static readonly string HexChars = "0123456789abcdef";
    public static readonly string Base32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";  // Base32 encoding - in ascii sort order https://datatracker.ietf.org/doc/html/rfc2938
    public static readonly string Base64Chars = "+/0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";  // Base64 encoding - in ascii sort order https://base64.guru/learn/base64-characters
    public static readonly string LowerCaseAlphaNumericChars = "0123456789abcdefghijklmnopqrstuvwxyz";
    public static readonly string UpperCaseAlphaNumericChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static readonly string AlphaNumericChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    public static readonly string SymbolsChars = "®@+-*=|°¬¿?¡!\"'`´¨~^#$%&<>()[]{}_.:,;/\\";
    public static readonly string IdentifierChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
    public static readonly string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz@+-*|?!\"´'#$%&/<>()[]{}_.:,;=/\\ ";
    public static readonly string WordSeparators = " .,";

    public static readonly Lazy<SearchValues<char>> SearchSymbolsChars = new Lazy<SearchValues<char>>(() => SearchValues.Create(StringExtensions.SymbolsChars));

    // public static Encoding Latin1Encoding = Encoding.GetEncoding(1252);   // Encoding.Latin1
    // public static Encoding CodePage437Encoding = Encoding.GetEncoding(437); // https://en.wikipedia.org/wiki/Code_page_437

    /// <summary>
    /// <see cref="Encoding.UTF8"/> without the preamble.
    /// </summary>
    /// <remarks>
    /// Preamble = UTF8 Identifier = Byte Order Mark (BOM)
    /// <see cref="Encoding.UTF8"/> has BOM.
    /// The Unicode Standard neither requires nor recommends the use of the BOM for UTF-8
    /// </remarks>
    /// <see cref="Encoding.GetPreamble"/>
    /// <see cref="https://en.wikipedia.org/wiki/UTF-8#Byte_order_mark"/>
    public static UTF8Encoding UTF8Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    public static string GetUTF8String(this byte[] buffer, int startIndex = 0)
        {
        int endIndex = Array.IndexOf<byte>(buffer, 0, startIndex);
        return StringExtensions.UTF8Encoding.GetString(buffer, startIndex, endIndex < 0? buffer.Length: endIndex);
        }

    public static string ToBase64String(this string input)
        {
        using PooledMemory<byte> inputBuffer = new PooledMemory<byte>(input.Length * 2);
        int inputLength = StringExtensions.UTF8Encoding.GetBytes(input, inputBuffer.Memory.Span);
        return Convert.ToBase64String(inputBuffer.Memory.Span[..inputLength]);
        }

    public static string CreateString(this IEnumerable<char> chars, int length)
        {
        Span<char> span = stackalloc char[length];
        chars.WriteTo(span);
        return new string(span);
        }

    /// <summary>
    /// Quita los tildes de las vocales y la ñ.
    /// </summary>
    /// <see cref="https://stackoverflow.com/a/5459722/5301336"/>
    /// <see cref="http://archives.miloush.net/michkap/archive/2007/05/14/2629747.html"/>
    public static string RemoveDiacritics(this string input)
        => input
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .CreateString(input.Length);

    public static bool IsEmpty(string value)
        => value.Length == 0;

    public static string IfIsNullOrEmpty(string? value, string defaultValue)
        => String.IsNullOrEmpty(value) ? defaultValue : value;

    /*
    /// <summary>
    /// Obtiene la subcadena entre leftToken y rightToken.
    /// </summary>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="leftToken">Token izquierdo.</param>
    /// <param name="rightToken">Token derecho.</param>
    /// <returns>Cadena obtenida.</returns>
    public static string GetTextBetweenTokens(this string str, string leftToken, string rightToken)
        {
        int left = str.IndexOf(leftToken);
        int right;

        if (left > 0)
            {
            left += leftToken.Length;
            right = str.IndexOf(rightToken, left);
            if (right > 0)
                {
                return str.Substring(left, right - left);
                }
            }

        return "";
        }
    */

    /// <summary>
    /// Enumera las subcadenas entre leftToken y rightToken.
    /// </summary>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="leftToken">Token izquierdo, si es vacio se toma desde el comienzo de la cadena.</param>
    /// <param name="rightToken">Token derecho, si es vacio se toma hasta el final de la cadena.</param>
    /// <returns>Cadenas obtenidas.</returns>
    public static IEnumerable<string> GetAllTextBetweenTokens(this string str, string leftToken, string rightToken)
        {
        int textIndex = 0;
        string? text;

        while ((text = StringExtensions.GetTextBetweenTokens(str, leftToken, rightToken, ref textIndex)) != null)
            {
            yield return text;
            }
        }

    /// <summary>
    /// Obtiene la subcadena entre leftToken y rightToken, incluyéndolos.
    /// </summary>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="leftToken">Token izquierdo, si es vacio se toma desde el comienzo de la cadena.</param>
    /// <param name="rightToken">Token derecho, si es vacio se toma hasta el final de la cadena.</param>
    /// <returns>Cadena obtenida con los tokens.</returns>
    public static string GetTextBetweenTokensIncluded(this string str, string leftToken, string rightToken)
        => String.Concat(leftToken, StringExtensions.GetTextBetweenTokens(str, leftToken, rightToken), rightToken);

    /// <summary>
    /// Obtiene la subcadena entre leftToken y rightToken.
    /// </summary>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="leftToken">Token izquierdo, si es vacio se toma desde el comienzo de la cadena.</param>
    /// <param name="rightToken">Token derecho, si es vacio se toma hasta el final de la cadena.</param>
    /// <returns>Cadena obtenida o vacio si no se encuentran los tokens.</returns>
    public static string GetTextBetweenTokens(this string str, string leftToken, string rightToken)
        {
        int zeroIndex = 0;
        return StringExtensions.GetTextBetweenTokens(str, leftToken, rightToken, ref zeroIndex) ?? "";
        }

    /// <summary>
    /// Obtiene la subcadena entre leftToken y rightToken.
    /// </summary>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="leftToken">Token izquierdo, si es vacio se toma desde el comienzo de la cadena.</param>
    /// <param name="rightToken">Token derecho, si es vacio se toma hasta el final de la cadena.</param>
    /// <param name="textIndex">Posición a partir de la cual comienza la busqueda.</param>
    /// <returns>Cadena obtenida o null si no se encuentran los tokens.</returns>
    public static string? GetTextBetweenTokens(this string str, string leftToken, string rightToken, ref int textIndex)
        {
        int leftTokenIndex = str.IndexOf(leftToken, textIndex, StringComparison.Ordinal);

        if (leftTokenIndex >= 0)
            {
            leftTokenIndex += leftToken.Length;

            if (rightToken == "")
                {
                textIndex = str.Length;
                return str[leftTokenIndex..];
                }

            int rightTokenIndex = str.IndexOf(rightToken, leftTokenIndex, StringComparison.Ordinal);
            if (rightTokenIndex > 0)
                {
                textIndex = rightTokenIndex + rightToken.Length;
                return str[leftTokenIndex..rightTokenIndex];
                }
            }

        return null;
        }

    /// <summary>
    /// Convierte la cadena en un boolean.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <remarks>Si la cadena no puede convertirse dispara una excepción.</remarks>
    public static bool ParseBoolean(this string str)
        {
        str = str.Trim();

        if (StringExtensions.EqualsIgnoringCase(str, "true") || StringExtensions.EqualsIgnoringCase(str, "t") || str == "1" || str == "-1")
            {
            return true;
            }

        if (StringExtensions.EqualsIgnoringCase(str, "false") || StringExtensions.EqualsIgnoringCase(str, "f") || str == "0")
            {
            return false;
            }

        throw new FormatException($"String \"{str}\" cannot be parsed as bool.");
        }

    /// <summary>
    /// Convierte la cadena en un decimal.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <returns>Decimal convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse null, no dispara una excepción.</remarks>
    public static decimal? TryParseNullableDecimal(this string str)
        => decimal.TryParse(str, out decimal outValue) ? outValue : (decimal?)null;

    /// <summary>
    /// Convierte la cadena en un int.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <returns>int convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna null, no dispara una excepción.</remarks>
    public static int? TryParseNullableInt(this string str)
        => int.TryParse(str, out int outValue) ? outValue : (int?)null;

    /// <summary>
    /// Convierte la cadena en un Long.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <returns>Long convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna 0, no dispara una excepción.</remarks>
    public static long? TryParseNullableLong(this string str)
        => long.TryParse(str, out long outValue) ? outValue : (long?)null;

    /// <summary>
    /// Convierte la cadena en un DateTime.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <returns>DateTime convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna DateTime.MinValue, no dispara una excepción.</remarks>
    public static DateTime? TryParseNullableDateTime(this string str)
        => DateTime.TryParse(str, out DateTime outValue) ? outValue : (DateTime?)null;

    /// <summary>
    /// Convierte la cadena en un decimal.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <param name="defaultValue"></param>
    /// <returns>Decimal convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna defaultValue, no dispara una excepción.</remarks>
    public static decimal TryParseDecimal(this string str, decimal defaultValue = 0M)
        => decimal.TryParse(str, out decimal outValue) ? outValue : defaultValue;

    /// <summary>
    /// Convierte la cadena en un int.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <param name="defaultValue"></param>
    /// <returns>int convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna defaultValue, no dispara una excepción.</remarks>
    public static int TryParseInt(this string str, int defaultValue = 0)
        => int.TryParse(str, out int outValue) ? outValue : defaultValue;

    /// <summary>
    /// Convierte la cadena en un Long.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <param name="defaultValue"></param>
    /// <returns>Long convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna defaultValue, no dispara una excepción.</remarks>
    public static long TryParseLong(this string str, long defaultValue = 0L)
        => long.TryParse(str, out long outValue) ? outValue : defaultValue;

    /// <summary>
    /// Convierte la cadena en un DateTime.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <returns>DateTime convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna DateTime.MinValue, no dispara una excepción.</remarks>
    public static DateTime TryParseDateTime(this string str)
        => str.TryParseDateTime(DateTime.MinValue);

    /// <summary>
    /// Convierte la cadena en un DateTime.
    /// </summary>
    /// <returns>DateTime convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna defaultValue, no dispara una excepción.</remarks>
    public static DateTime TryParseDateTime(this string str, DateTime defaultValue)
        => DateTime.TryParse(str, out DateTime outValue) ? outValue : defaultValue;

    /// <summary>
    /// Convierte la cadena en un DateTime.
    /// </summary>
    /// <returns>DateTime convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna defaultValue, no dispara una excepción.</remarks>
    public static DateTime TryParseDateTime(this string str, string format, DateTime defaultValue)
        => DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime outValue) ? outValue : defaultValue;

    /// <summary>
    /// Convierte la cadena en un Enum.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <param name="ignoreCase">true to ignore case; false to consider case.</param>
    /// <returns>int convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna 0, no dispara una excepción.</remarks>
    public static TEnum TryParseEnum<TEnum>(this string str, bool ignoreCase = false) where TEnum : struct
        {
        Enum.TryParse(str, ignoreCase, out TEnum outValue);
        return outValue;
        }

    /// <summary>
    /// Convierte la cadena en un Enum.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <param name="defaultValue"></param>
    /// <param name="ignoreCase">true to ignore case; false to consider case.</param>
    /// <returns>int convertido.</returns>
    /// <remarks>Si la cadena no puede convertirse retorna defaultValue, no dispara una excepción.</remarks>
    public static TEnum TryParseEnum<TEnum>(this string str, TEnum defaultValue, bool ignoreCase = false) where TEnum : struct
        => Enum.TryParse(str, ignoreCase, out TEnum outValue) ? outValue : defaultValue;

    /// <summary>
    /// Convierte la cadena en un entero menor a 256.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <remarks>Si la cadena no puede convertirse dispara una excepción.</remarks>
    public static void ParseByte(this string str) => byte.Parse(str);

    /// <summary>
    /// Convierte la cadena en un entero.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <remarks>Si la cadena no puede convertirse dispara una excepción.</remarks>
    public static void ParseInt(this string str) => int.Parse(str);

    /// <summary>
    /// Convierte la cadena en un entero largo.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <remarks>Si la cadena no puede convertirse dispara una excepción.</remarks>
    public static void ParseLong(this string str) => long.Parse(str);

    /// <summary>
    /// Convierte la cadena en un decimal.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <remarks>Si la cadena no puede convertirse dispara una excepción.</remarks>
    public static void ParseDecimal(this string str) => decimal.Parse(str);

    /// <summary>
    /// Convierte la cadena en un DateTime.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <remarks>Si la cadena no puede convertirse dispara una excepción.</remarks>
    public static void ParseDateTime(this string str) => DateTime.Parse(str);

    /// <summary>
    /// Convierte la cadena en un DateTime, sin hora, con formato dd/MM/yyyy.
    /// </summary>
    /// <param name="str">Cadena a parsear</param>
    /// <remarks>Si la cadena no puede convertirse dispara una excepción.</remarks>
    public static void ParseDate(this string str) => DateTime.ParseExact(str, "dd/MM/yyyy", CultureInfo.InvariantCulture);

    /// <summary>
    /// Concatena las dos cadenas.
    /// </summary>
    /// <remarks>
    /// Para usar en expresiones linq.
    /// </remarks>
    public static string Concat2(this string str, string value)
        => String.Concat(str, value);

    /// <summary>
    /// Inserta una cadena dentro de otra.
    /// </summary>
    /// <remarks>A diferencia de String.Insert() no da error si startIndex esta fuera de los limites de strTarget</remarks>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="value">Cadena a insertar.</param>
    /// <param name="startIndex">
    /// 0: inicio de la cadena
    /// n: posicion n de la cadena
    /// -n: posicion largo-n de la cadena
    /// </param>
    /// <returns>Cadena procesada.</returns>
    public static string Insert2(this string str, string value, int startIndex = 0)
        {
        if (value == "")
            {
            return str;
            }

        if (str == "")
            {
            return value;
            }

        if (startIndex > str.Length)
            {
            return String.Concat(str, value);
            }

        if (startIndex == 0 || startIndex < -str.Length)
            {
            return String.Concat(value, str);
            }

        if (startIndex < 0)
            {
            startIndex += str.Length;
            }

        return str.Insert(startIndex, value);
        }

    /// <summary>
    /// Borra una parte de una cadena.
    /// </summary>
    /// <remarks>
    /// A diferencia de String.Remove() no da error si startIndex o count estan fuera de los limites inferior o superior de str
    /// </remarks>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="startIndex">
    /// 0: inicio de la cadena
    /// n: posicion n de la cadena
    /// -n: posicion largo-n de la cadena
    /// </param>
    /// <param name="count">
    /// 0: hasta el final de la cadena
    /// n: n carácteres delante de StartIndex
    /// -n: n carácteres detrás de StartIndex
    /// </param>
    /// <returns>Cadena procesada.</returns>
    public static string Remove2(this string str, int startIndex = 0, int count = 0)
        {
        if (str == "")
            {
            return "";
            }

        // ajusto startIndex, < 0 indica desde el final
        if (startIndex < 0)
            {
            startIndex += str.Length;
            }

        // count == 0 indica hasta el final
        if (count == 0)
            {
            return startIndex < 0 ? "" : startIndex >= str.Length ? str : str.Remove(startIndex);
            }

        // count < 0 indica desde startIndex hacia atrás
        if (count < 0)
            {
            startIndex += count;
            count = Math.Abs(count);

            if (startIndex < 0)
                {
                count += startIndex;
                startIndex = 0;
                }
            }

        return startIndex <= 0 && startIndex + count >= str.Length
            ? ""
            : startIndex >= str.Length || count <= 0 || startIndex + count <= 0
            ? str
            : startIndex + count > str.Length
            ? str.Remove(startIndex)
            : str.Remove(startIndex, count);
        }

    /// <summary>
    /// Reemplaza una parte de una cadena por otra.
    /// </summary>
    /// <remarks>
    /// A diferencia de String.Replace() no da error si str == "" o oldValue es null o "".
    /// </remarks>
    public static string Replace2(this string str, string? oldValue, string newValue)
        => str == "" || String.IsNullOrEmpty(oldValue) ? str : str.Replace(oldValue, newValue);

    /// <summary>
    /// Similar a String.Substring(), pero no da error cuando los parametros estan fuera del rango de la cadena.
    /// </summary>
    /// <param name="str">Cadena a examinar.</param>
    /// <param name="startIndex">
    /// 0: inicio de la cadena
    /// n: posicion n de la cadena
    /// -n: posicion largo-n de la cadena (desde el final)
    /// </param>
    /// <param name="length">
    /// 0: hasta el final de la cadena
    /// n: n carácteres delante de StartIndex
    /// -n: n carácteres detrás de StartIndex
    /// </param>
    /// <remarks>
    /// Si str es "" retorna "".
    /// </remarks>
    /// <returns>Subcadena obtenida.</returns>
    public static string Substring2(this string str, int startIndex = 0, int length = 0)
        {
        if (str == "")
            {
            return "";
            }

        // ajusto startIndex, < 0 indica desde el final
        if (startIndex < 0)
            {
            startIndex += str.Length;
            }

        // length == 0 indica hasta el final
        if (length == 0)
            {
            return startIndex < 0 ? str : startIndex >= str.Length ? "" : str.Substring(startIndex);
            }

        // length < 0 indica desde startIndex hacia atrás
        if (length < 0)
            {
            startIndex += length;
            length = Math.Abs(length);

            if (startIndex < 0)
                {
                length += startIndex;
                startIndex = 0;
                }
            }

        return startIndex <= 0 && startIndex + length >= str.Length
            ? str
            : startIndex >= str.Length || length <= 0 || startIndex + length <= 0
            ? ""
            : startIndex + length > str.Length
            ? str.Substring(startIndex)
            : str.Substring(startIndex, length);
        }

    /// <summary>
    /// Retorna la subcadena entre los indices (inclusive).
    /// </summary>
    /// <param name="str">Cadena a examinar.</param>
    /// <param name="startIndex">
    /// 0: inicio de la cadena
    /// n: posicion n de la cadena
    /// -n: posicion largo-n de la cadena (-1: final de la cadena)
    /// </param>
    /// <param name="endIndex">
    /// 0: inicio de la cadena
    /// n: posicion n de la cadena
    /// -n: posicion largo-n de la cadena (-1: final de la cadena)
    /// </param>
    /// <remarks>No da error cuando los parametros estan fuera del rango de la cadena.</remarks>
    /// <returns>Subcadena obtenida.</returns>
    public static string SubstringBetweenIndexes(this string str, int startIndex = 0, int endIndex = -1)
        {
        if (str == "")
            {
            return "";
            }

        // ajusto startIndex, < 0 indica desde el final
        if (startIndex < 0)
            {
            startIndex += str.Length;
            }

        // ajusto endIndex, <= 0 indica desde el final
        if (endIndex < 0)
            {
            endIndex += str.Length;
            }

        // si los indices estan invertidos, se dan vuelta
        if (startIndex > endIndex)
            {
            ObjectExtensions.Swap(ref startIndex, ref endIndex);
            }

        // si los indices estan fuera de la cadena, se retorna vacio
        if (startIndex >= str.Length || endIndex < 0)
            {
            return "";
            }

        // ajusto startIndex
        if (startIndex < 0)
            {
            startIndex = 0;
            }

        int length = endIndex - startIndex + 1;

        return length <= 0 ? "" : startIndex + length > str.Length ? str.Substring(startIndex) : str.Substring(startIndex, length);
        }

    /// <summary>
    /// Converts a string to its escaped representation.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="spaceAsPlus">Si es true, encodea los espacios con '+'</param>
    /// <returns>Escaped representation</returns>
    /// <remarks>
    /// EscapeURLDataString replaces spaces with "+" signs, and unsafe ASCII characters
    /// with "%" followed by their hex equivalent. Safe characters are defined in RFC2396.
    /// They are the 7-bit ASCII alphanumerics and the mark characters "-_.!~*'()".
    /// (The standard Uri.EscapeDataString() operate slightly differently: 
    /// it encode space as "%20", and treat "+" as a safe character.)
    /// x-www-urlencoded
    /// </remarks>
    public static string EncodeUrl(this string url, bool spaceAsPlus = false)
        => spaceAsPlus ? HttpUtility.UrlEncode(url) : Uri.EscapeDataString(url);

    /// <summary>
    /// Converts a string to its unescaped representation.
    /// </summary>
    /// <param name="escapedUrl"></param>
    /// <param name="spaceAsPlus">Si es true, decodea los '+' como espacio </param>
    /// <returns>Unescaped representation</returns>
    public static string DecodeUrl(this string url, bool spaceAsPlus = false)
        => spaceAsPlus ? HttpUtility.UrlDecode(url) : Uri.UnescapeDataString(url);

    public static Uri? TryCreateUri(this string value, UriKind uriKind = UriKind.Absolute)
        => Uri.TryCreate(value, uriKind, out Uri? uri) ? uri : null;

    /// <summary>
    /// Converts a string to its escaped representation.
    /// </summary>
    /// <returns>Escaped representation</returns>
    public static string EscapeCString(this string str)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();

        foreach (char c in str)
            {
            switch (c)
                {
                case '\a': builder.StringBuilder.Append('\\').Append('a'); break;        // Matches a bell (alarm) \u0007
                case '\b': builder.StringBuilder.Append('\\').Append('b'); break;        // Matches a backspace \u0008
                case '\f': builder.StringBuilder.Append('\\').Append('f'); break;        // Matches a form feed \u000C
                case '\n': builder.StringBuilder.Append('\\').Append('n'); break;        // Matches a new line \u000A
                case '\r': builder.StringBuilder.Append('\\').Append('r'); break;        // Matches a carriage return \u000D
                case '\t': builder.StringBuilder.Append('\\').Append('t'); break;        // Matches a tab \u0009
                case '\v': builder.StringBuilder.Append('\\').Append('v'); break;        // Matches a vertical tab \u000B
                case '\"': builder.StringBuilder.Append('\\').Append('\"'); break;
                case '\'': builder.StringBuilder.Append('\\').Append('\''); break;
                case '\\': builder.StringBuilder.Append('\\').Append('\\'); break;

                default: builder.StringBuilder.Append(c); break;
                }
            }

        return builder.StringBuilder.ToString();
        }

    /// <summary>
    /// Converts a string to its unescaped representation.
    /// </summary>
    /// <returns>Unescaped representation</returns>
    public static string UnescapeCString(this string escapedStr)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        bool escaping = false;
        foreach (char c in escapedStr)
            {
            if (escaping)
                {
                switch (c)
                    {
                    case 'a': builder.StringBuilder.Append('\a'); break;        // Matches a bell (alarm) \u0007
                    case 'b': builder.StringBuilder.Append('\b'); break;        // Matches a backspace \u0008
                    case 'f': builder.StringBuilder.Append('\f'); break;        // Matches a form feed \u000C
                    case 'n': builder.StringBuilder.Append('\n'); break;        // Matches a new line \u000A
                    case 'r': builder.StringBuilder.Append('\r'); break;        // Matches a carriage return \u000D
                    case 't': builder.StringBuilder.Append('\t'); break;        // Matches a tab \u0009
                    case 'v': builder.StringBuilder.Append('\v'); break;        // Matches a vertical tab \u000B
                    case '\"': builder.StringBuilder.Append('"'); break;
                    case '\'': builder.StringBuilder.Append('\''); break;
                    case '\\': builder.StringBuilder.Append('\\'); break;

                    /*
                    If the character following the backslash does not specify a legal escape sequence, 
                    the result is implementation defined. In Microsoft C++, the character 
                    following the backslash is taken literally, as though the escape were not present.
                    */
                    default: builder.StringBuilder.Append(c); break;
                    }
                escaping = false;
                }
            else if (!(escaping = c == '\\'))
                {
                builder.StringBuilder.Append(c);
                }
            }

        return builder.StringBuilder.ToString();
        }

    /// <summary>
    /// Idem String.Equals pero ignorando minusculas/mayusculas en la busqueda.
    /// </summary>
    public static bool EqualsIgnoringCase(this string input, string? value)
        => input.Equals(value, StringComparison.InvariantCultureIgnoreCase);

    public static bool EqualsInvariantCulture(this string input, string? value)
        => input.Equals(value, StringComparison.InvariantCulture);

    public static int CompareInvariantCulture(this string input, string? value)
        => String.Compare(input, value, StringComparison.InvariantCulture);

    /// <summary>
    /// Idem String.Contains pero ignorando minusculas/mayusculas en la busqueda.
    /// </summary>
    /// <see cref="https://stackoverflow.com/a/444818/15464440"/>
    public static bool ContainsIgnoringCase(this string input, string value)
        => input.Contains(value, StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Idem String.Contains pero permite ignorar minusculas/mayusculas en la busqueda.
    /// </summary>
    /// <see cref="https://stackoverflow.com/a/15464440/11333"/>
    public static bool Contains(this string input, string search, CompareOptions options = CompareOptions.None)
        => input.Contains(search, CultureInfo.InvariantCulture, options);

    /// <summary>
    /// Idem String.Contains pero permite ignorar minusculas/mayusculas segun una cultura en la busqueda.
    /// </summary>
    /// <see cref="https://stackoverflow.com/a/15464440/11333"/>
    public static bool Contains(this string input, string search, CultureInfo cultureInfo, CompareOptions options = CompareOptions.None)
        => cultureInfo.CompareInfo.IndexOf(input, search, options) >= 0;

    /// <summary>
    /// Elimina los acentos, diéresis y ñ (reemplazándolos por sus caracteres equivalentes).
    /// </summary>
    /// <param name="sentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string RemoveAccents(this string sentence)
        => sentence
            .Replace('ñ', 'n')
            .Replace('á', 'a')
            .Replace('é', 'e')
            .Replace('í', 'i')
            .Replace('ó', 'o')
            .Replace('ú', 'u')
            .Replace('ü', 'u')
            .Replace('Ñ', 'N')
            .Replace('Á', 'A')
            .Replace('É', 'E')
            .Replace('Í', 'I')
            .Replace('Ó', 'O')
            .Replace('Ú', 'U')
            .Replace('Ü', 'U')
            ;

    /// <summary>
    /// Convierte la cadena a "Tipo oración".
    /// </summary>
    /// <remarks>Si el primer caracter no es una letra, la cadena no quedará en mayúsculas.</remarks>
    /// <param name="sentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string Capitalize(this string sentence)
        => String.Concat(Char.ToUpperInvariant(sentence[0]), sentence[1..].ToLowerInvariant());

    /// <summary>
    /// Convierte la cadena a "minúsculas".
    /// </summary>
    /// <remarks>Si el primer caracter no es una letra, la cadena no quedará en minúsculas.</remarks>
    /// <param name="sentence">Cadena a convertir</param>
    /// <returns>Cadena convertida.</returns>
    public static string Decapitalize(this string sentence)
        => String.Concat(Char.ToLowerInvariant(sentence[0]), sentence[1..]);

    /// <summary>
    /// Concatena todas las cadenas de la secuencia.
    /// </summary>
    public static string Join(this IEnumerable<string> items)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        return items
                .Aggregate(builder.StringBuilder, (stringBuilder, item) => stringBuilder.Append(item))
                .ToString();
        }

    /// <summary>
    /// Concatena todas las cadenas de la secuencia.
    /// </summary>
    public static string JoinLines(this IEnumerable<string> items)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        return items
                .Aggregate(builder.StringBuilder, (stringBuilder, item) => stringBuilder.AppendLine(item))
                .ToString();
        }

    public static string[] Split(this string value, string separator, int count)
        => StringExtensions.Split(value, separator, count, StringSplitOptions.None);

    /// <summary>
    ///     Returns a System.String array containing the substrings in this string that
    ///     are delimited by elements of a specified System.Char array. Parameters specify
    ///     the maximum number of substrings to return and whether to return empty array
    ///     elements.
    /// </summary>
    /// <remarks>Idem String.Split() pero el ultimo item tambien es parseado (no tiene el resto de la cadena)</remarks>
    /// <param name="value">This string instance.</param>
    /// <param name="separator">
    ///     An array of Unicode characters that delimit the substrings in this string,
    ///     an empty array containing no delimiters, or null.
    /// </param>
    /// <param name="count">The maximum number of substrings to return.</param>
    /// <param name="options">
    ///     Specify System.StringSplitOptions.RemoveEmptyEntries to omit empty array
    ///     elements from the array returned, or System.StringSplitOptions.None to include
    ///     empty array elements in the array returned.
    /// </param>
    /// <returns>
    ///     An array whose elements contain the substrings in this stringthat are delimited
    ///     by one or more characters in separator. For more information, see the Remarks
    ///     section.
    /// </returns>
    /// <exception cref="System.ArgumentException">options is not one of the System.StringSplitOptions values.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">count is negative.</exception>
    public static string[] Split(this string value, string separator, int count, StringSplitOptions options)
        {
        string[] tempArray = value.Split([separator], count + 1, options);
        Array.Resize(ref tempArray, count);
        return tempArray;
        }

    public static string[] Split(this string value, string separator)
        => value.Split(separator, StringSplitOptions.None);

    public static string[] Split(this string value, string separator, StringSplitOptions options)
        => value.Split([separator], options);

    public static string AlignOrTruncate(this string str, int width, char paddingChar = ' ', Aligning aligning = Aligning.None)
        => aligning switch
            {
            Aligning.Left => StringExtensions.AlignLeftOrTruncate(str, width, paddingChar),
            Aligning.Right => StringExtensions.AlignRightOrTruncate(str, width, paddingChar),
            Aligning.Center => StringExtensions.Center(str, width, paddingChar),
            Aligning.Justify => RegexExtensions.Justify(str, width, paddingChar),
            _ => StringExtensions.Substring2(str, 0, width),
            };

    /// <summary>
    ///  Alinea a la izquierda los caracteres de la cadena, completando con el caracter especificado
    ///  a la derecha, para completar el ancho total especificado o trunca la cadena
    ///  si es mas larga que dicho ancho.
    /// </summary>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="width">Largo de la cadena retornada.</param>
    /// <param name="paddingChar">Caracter con el que completar.</param>
    /// <returns>Cadena completada o truncada.</returns>
    public static string AlignLeftOrTruncate(this string str, int width, char paddingChar = ' ')
        => str.PadRight(width, paddingChar).Substring(0, width);

    /// <summary>
    ///  Alinea a la derecha los caracteres de la cadena, completando con el caracter especificado
    ///  a la izquierda, para completar el ancho total especificado o trunca la cadena
    ///  si es mas larga que dicho ancho.
    /// </summary>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="width">Largo de la cadena retornada.</param>
    /// <param name="paddingChar"></param>
    /// <returns>Cadena completada o truncada.</returns>
    /// <see cref="http://msdn.microsoft.com/en-us/library/dd260048(v=vs.110).aspx"/>
    public static string AlignRightOrTruncate(this string str, int width, char paddingChar = ' ')
        => str.Length == width ? str : (str.Length < width ? str.PadLeft(width, paddingChar) : str[^width..]);

    /// <summary>
    ///  Centra los caracteres de la cadena, completando con el caracter especificado
    ///  a la derecha e izquierda, para completar el ancho total especificado
    ///  o trunca la cadena si es mas larga que dicho ancho.
    /// </summary>
    /// <param name="str">Cadena a procesar.</param>
    /// <param name="width">Largo de la cadena retornada.</param>
    /// <param name="paddingChar">Caracter con el que completar.</param>
    /// <returns>Cadena completada o truncada.</returns>
    public static string Center(this string str, int width, char paddingChar = ' ')
        {
        int midWidth = (int)Math.Ceiling((width - str.Length) / 2M);
        if (midWidth > 0)
            {
            string midString = new String(paddingChar, midWidth);
            str = String.Concat(midString, str, midString);
            }
        return str.Substring2(0, width);
        }

    /// <summary>
    /// Obtiene todos los digitos de la cadena (descartando el resto).
    /// </summary>
    public static string GetDigits(this string input)
    // => Regex.Replace(input, @"\D", "");
        => StringExtensions.GetChars(input, Char.IsDigit);

    /// <summary>
    /// Obtiene todos los caracteres de la cadena que satisfacen el predicado (descartando el resto).
    /// </summary>
    public static string GetChars(this string input, Func<char, bool> predicate) 
        => input.Where(predicate).CreateString(input.Length);

    /// <summary>
    /// Reemplaza todos los caracteres especificados por el nuevo caracter
    /// </summary>
    public static string ReplaceChars(this string str, IReadOnlyCollection<char> oldChars, char newChar)
        //  => oldChars.Aggregate(str, (s, oldChar) => s.Replace(oldChar, newChar));                        // recorre oldChars y por cada uno recorre str
        //  => new String(str.ToCharEnumerable().Select(c => oldChars.Contains(c)? newChar: c).ToArray());  // recorre str y por cada uno recorre removeChars
        => str
            .Select((oldChars, newChar), (char c, (IReadOnlyCollection<char> OldChars, char NewChar) par) => par.OldChars.Contains(c) ? par.NewChar : c)
            .CreateString(str.Length);        

    /// <summary>
    /// Quita todos los caracteres especificados
    /// </summary>
    public static string RemoveChars(this string str, IReadOnlyCollection<char> removeChars)
        //    => removeChars.Aggregate(strTarget, (s, removeChar) => s.RemoveChar(removeChar));                     // recorre removeChars y por cada uno recorre str
        => new String(str.Where(removeChars, (c, removeCs) => !removeCs.Contains(c)).ToArray());                // recorre str y por cada uno recorre removeChars

    /// <summary>
    /// Reemplaza el caracter especificado por el nuevo caracter
    /// </summary>
    /// <remarks>Use String.Replace(char oldChar, char newChar)</remarks>
    //public static string ReplaceChar(this string str, char oldChar, char newChar)
    //    => new String(str.ToCharEnumerable().Select(c => c == oldChar? newChar: c).ToArray());

    /// <summary>
    /// Quita el caracter especificado
    /// </summary>
    public static string RemoveChar(this string str, char removeChar)
        => new String(str.Where(removeChar, (c, removeC) => c != removeC).ToArray());

    public static string ShortenString(this string str, int length)
        => str.Length > length ? String.Concat(str.Substring(0, length - 1), "…") : str;

    /// <summary>
    /// Obtiene un valor de la linea de comandos.
    /// </summary>
    /// <remarks>
    /// Los items de args deben tener el formato:
    /// key=value
    /// key="other value"
    /// </remarks>
    /// <returns>El valor de la clave de linea de comandos especificada o null si no se encuentra.</returns>
    /// <see cref="https://msdn.microsoft.com/en-us/magazine/mt833289"/>
    /// <see cref="https://github.com/dotnet/command-line-api"/>
    public static string? GetCommandLineArgValue(this IEnumerable<string> args, string key)
        => args
            .Select(arg => arg.Split(['='], 2))
            .Where(key, (arr, par) => arr[0] == par)
            .Select(arr => arr[1].RemoveQuotes())
            .FirstOrDefault();

    /// <summary>
    /// Parsea los argumentos de la linea de comandos.
    /// </summary>
    /// <remarks>
    /// Los items de args deben tener el formato:
    /// key=value
    /// key="other value"
    /// </remarks>
    /// <returns>Diccionario con los argumentos y valores encontrados.</returns>
    /// <see cref="https://msdn.microsoft.com/en-us/magazine/mt833289"/>
    /// <see cref="https://github.com/dotnet/command-line-api"/>
    public static Dictionary<string, string> ParseCommandLineArgs(this IEnumerable<string> args)
        => args
            .Select(arg => arg.Split(['='], 2))
            .ToDictionary(arr => arr[0], arr => arr[1].RemoveQuotes());

    public static string RemoveQuotes(this string input)
        => input != "" && input.Length >= 2 && input[0] == '"' && input[^1] == '"' ?
                input[1..^1] :
                input;

    public static string ToArgentineCurrency(this decimal num)
        => num.ToString("C", CultureInfo.GetCultureInfo("es-AR"));

    public static bool IsNullOrEmptyOrWhiteSpace(string value)
        => value == "" || String.IsNullOrWhiteSpace(value);

    public static IEnumerable<string> ReadLines(this string value)
        {
        string? line;
        using StringReader stringReader = new StringReader(value);
        while ((line = stringReader.ReadLine()) != null)
            {
            yield return line;
            }
        }

    public static string[] ReadWords(this string line)
        => line.Split(StringExtensions.WordSeparators.AsSpan());

    public static IEnumerable<int> ToIntEnumerable(this string value)
        => value.Select(c => c - '0');

    public static int[] ToIntArray(this string value)
        => value.ToIntEnumerable().ToArray(value.Length);

    public static bool IsAlphaNumeric(this string value)
        => !String.IsNullOrEmpty(value) && value.All(StringExtensions.AlphaNumericChars.Contains);

    public static bool ContainsAllAsciiChar(this string value)
        => value.All(Char.IsAscii);

    public static bool ContainsSymbols(this string value)
        //=> value.Any(StringExtensions.SymbolsChars.Contains);
        //=> value.ContainsAnyChar(StringExtensions.SymbolsChars);
        => value.ContainsAnyChar(StringExtensions.SearchSymbolsChars.Value);

    public static bool ContainsAnyChar(this string value, params ReadOnlySpan<char> chars)
        => value.ContainsAnyChar(SearchValues.Create(chars));

    public static bool ContainsAnyChar(this string value, SearchValues<char> searchChars)
        => value.AsSpan().ContainsAny(searchChars);

    public static bool IsIdentifier(this string value)
        =>  value.Length > 0 && 
            !Char.IsDigit(value[0]) &&
            value.All(StringExtensions.IdentifierChars.Contains);

    public static string ValidateIdentifier(this string value)
        => value.IsIdentifier()? value: throw new InvalidOperationException($"Value is not a valid indentifier: {value}");
    }
