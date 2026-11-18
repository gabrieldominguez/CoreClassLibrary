using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects;
using CoreClassLibrary.Objects.Pooling;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Becas1.CoreClassLibrary.RegularExpressions.Templating;

public static partial class TemplatingExtensions
    {
    [GeneratedRegex("\\{(?<token>\\w+)(?<format>:[^}]+)?\\}", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex NamedFormatRegex();

    [GeneratedRegex("\\{(?<token>\\w+)(?<format>:[^}]+)?\\}", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex NumberedFormatRegex();

    public static string ToInvariantString(this object value)
        => value is DateTime dateTimeValue?
            dateTimeValue.ToLocalDateTimeString():
            String.Format(CultureInfo.InvariantCulture, "{0}", value);

    /// <summary>
    /// Reemplaza nombres de propiedades comenzando con un caracter (ej: '@', ':').
    /// </summary>
    /// <example>
    /// "@CurrentTime - @ProcessName" => "1975-11-12 00:00:00 - ABC"
    /// </example>
    public static string ParamReplace(this string format, object args, char parameterChar = '@', string nullString = "")
        => format.ParamReplace(args.GetPropertiesDictionary(), parameterChar, nullString);

    public static string ParamReplace(this string format, IDictionary<string, object?> dictionary, char parameterChar = '@', string nullString = "")
        => format.ParamReplace(dictionary.ToDictionary(pair => pair.Key, pair => pair.Value?.ToInvariantString() ?? nullString), parameterChar);

    public static string ParamReplace(this string format, IDictionary<string, string> dictionary, char parameterChar = '@')
        => format.RegexReplace($@"{parameterChar}(?<token>\w+)", "token", dictionary);

    /// <summary>
    /// Reemplaza nombres de propiedades entre llaves.
    /// </summary>
    /// <remarks>
    /// Reemplaza directamente las propiedades en la cadena, a diferencia de NamedFormat, no soporta formatos, 
    /// pero la cadena debe escanearse una sola vez.
    /// </remarks>
    /// <example>
    /// "{CurrentTime} - {ProcessName}" => "1975-11-12 00:00:00 - ABC"
    /// </example>
    public static string NamedReplace(this string format, object args, string nullString = "")
        => format.NamedReplace(args.GetPropertiesDictionary(), nullString);

    public static string NamedReplace(this string format, IDictionary<string, object?> dictionary, string nullString = "")
        => format.NamedReplace(dictionary.ToDictionary(pair => pair.Key, pair => pair.Value?.ToInvariantString() ?? nullString));

    public static string NamedReplace(this string format, IDictionary<string, string> dictionary)
        => format.RegexReplace(@"\{(?<token>\w+)\}", "token", dictionary);

    private static string RegexReplace(this string format, string pattern, string nameToken, IDictionary<string, string> dictionary)
        => Regex.Replace(
                       format,
                       pattern,
                       match => dictionary.TryGetValue(match.Groups[nameToken].Value, out string? value) ? value : match.Value,
                       RegexOptions.Compiled | RegexOptions.CultureInvariant);            

    /// <summary>
    /// Reemplaza nombres de propiedades en llaves aplicando String.Format.
    /// </summary>
    /// <remarks>
    /// La cadena es escaneada dos veces, la primera para convertir los nombres a numeros y la 2da en String.Format.
    /// </remarks>
    /// <example>
    /// "{CurrentTime:yyyy-MM-dd} - {ProcessName}" => "1975-11-12 - ABC"
    /// </example>
    /// <see cref="https://gist.github.com/atifaziz/43353"/>
    /// <see cref="http://james.newtonking.com/archive/2008/03/29/formatwith-2-0-string-formatting-with-named-variables"/>
    /// <see cref="http://haacked.com/archive/2009/01/04/fun-with-named-formats-string-parsing-and-edge-cases.aspx"/>
    /// <see cref="https://haacked.com/archive/2009/01/14/named-formats-redux.aspx/"/>
    public static string NamedFormat(this string format, object args, IFormatProvider? provider = null)
        {
        PropertyInfo[] properties = args.GetType().GetProperties();
        using PooledMemory<object?> propertiesValues = new PooledMemory<object?>(properties.Length);
        properties.Select(prop => prop.GetValue(args)).WriteTo(propertiesValues.Memory.Span);
        return string.Format(provider, format.ToNumberedFormat(properties), propertiesValues.Memory.Span);
        }

    public static string ToNumberedFormat<T>(this string format)
        => format.ToNumberedFormat(typeof(T));

    /// <summary>
    /// Convierte una cadena cuyas claves son el nombre de las propiedades de una clase a numeros correspondientes 
    /// a la posicion de la propiedad en la clase, para usar con string.Format().
    /// </summary>
    /// <example>
    /// "{CurrentTime:yyyy-MM-dd} - {ProcessName}" => "{0:yyyy-MM-dd} - {1}"
    /// </example>
    /// <remarks>
    /// En los tipos anonimos el orden en que se enumeran las propiedades no está garantizado, 
    /// ya sea con Type.GetProperties() o con TypeInfo.DeclaredProperties.
    /// </remarks>
    public static string ToNumberedFormat(this string format, Type type)
        => format.ToNumberedFormat(type.GetProperties());

    public static string ToNumberedFormat(this string format, IReadOnlyCollection<PropertyInfo> properties)
        {
        using PooledMemory<string> names = new PooledMemory<string>(properties.Count);
        properties.Select(prop => prop.Name).WriteTo(names.Memory.Span);
        return format.ToNumberedFormat(names.Memory);
        }

    public static string ToNumberedFormat(this string format, ReadOnlyMemory<string> names)
        => TemplatingExtensions.NumberedFormatRegex().Replace(format, match =>
            {
            int index = names.Span.IndexOf(match.Groups["token"].Value);
            return index == -1?
                match.Value:
                string.Concat("{", index.ToString(), match.Groups["format"].Value, "}");
            });

    public static string ToNamedFormat<T>(this string format)
        => format.ToNamedFormat(typeof(T));

    /// <summary>
    /// Convierte una cadena cuyas claves son la posicion de la propiedades de una clase,
    /// a los nombres de las mismas.
    /// </summary>
    /// <example>
    /// "{0:yyyy-MM-dd} - {1}" => "{CurrentTime:yyyy-MM-dd} - {ProcessName}"
    /// </example>
    /// <remarks>
    /// En los tipos anonimos el orden en que se enumeran las propiedades no está garantizado, 
    /// ya sea con Type.GetProperties() o con TypeInfo.DeclaredProperties.
    /// </remarks>
    public static string ToNamedFormat(this string format, Type type)
        => format.ToNamedFormat(type.GetProperties());

    public static string ToNamedFormat(this string format, IReadOnlyCollection<PropertyInfo> properties)
        {
        using PooledMemory<string> names = new PooledMemory<string>(properties.Count);
        properties.Select(prop => prop.Name).WriteTo(names.Memory.Span);
        return format.ToNamedFormat(names.Memory);
        }

    public static string ToNamedFormat(this string format, ReadOnlyMemory<string> names)
        => TemplatingExtensions.NamedFormatRegex().Replace(format, match =>
            {
            if (!int.TryParse(match.Groups["token"].Value, out int index))
                {
                return match.Value;
                }

            string? value = names.Span.GetValueOrDefault(index);
            return value == null?
                match.Value:
                string.Concat("{", value, match.Groups["format"].Value, "}");
            });
    }
