using System.Reflection;

namespace CoreClassLibrary.RegularExpressions.Templating;

/// <summary>
/// Permite utilizar las propiedades de un objecto en String.Format().
/// </summary>
/// <example>
/// String.Format(PropertyFormatProvider.Instance, "{0:CurrentTime} - {0:Data}", new { Data = data, CurrentTime = DateTime.Parse(datetime) })
/// "12/11/1975 00:00:00 - ABC"
/// </example>
/// <see cref="https://stackoverflow.com/questions/733378/whats-a-good-way-of-doing-string-templating-in-net"/>
public class PropertyFormatProvider : IFormatProvider, ICustomFormatter
{
    public static readonly PropertyFormatProvider Instance = new PropertyFormatProvider();

    public object GetFormat(Type? formatType)
        => this;

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        if (format == null)
        {
            return "";
        }

        PropertyInfo? property = arg?.GetType().GetProperty(format);
        return property != null ? property.GetValue(arg)?.ToInvariantString() ?? "" : format;
    }
}
