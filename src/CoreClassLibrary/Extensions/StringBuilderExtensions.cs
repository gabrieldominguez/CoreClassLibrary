using System.Text;
using System.Globalization;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;

namespace CoreClassLibrary.Extensions;

public static class StringBuilderExtensions
    {
    /// <summary>
    /// Agrega un indice a un formatString
    /// </summary>
    /// <example>
    /// "000".ToFormatString()
    /// "{0:000}"
    /// </example>
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.string.format?view=net-6.0#FormatItem"/>
    public static string ToFormatString(this string formatString, int index = 0)
        => String.Concat("{", index.ToString(), ":", formatString, "}");

    public static string RepeatAppend(this string value, int count)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        return builder.StringBuilder.RepeatAppend(value, count).ToString();
        }

    public static StringBuilder RepeatAppend(this StringBuilder stringBuilder, string value, int count)
        {
        using StringWriter writer = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);

        writer.RepeatWrite(value, count);
        writer.Flush();

        return stringBuilder;
        }

    public static StringBuilder AppendIf(this StringBuilder stringBuilder, bool condition, Func<object> factory)
        {
        if (condition)
            {
            stringBuilder.Append(factory());
            }
        return stringBuilder;
        }

    public static StringBuilder AppendLineIf(this StringBuilder stringBuilder, bool condition, Func<object> factory)
        {
        if (condition)
            {
            stringBuilder.AppendLine(factory());
            }
        return stringBuilder;
        }

    public static StringBuilder AppendIf(this StringBuilder stringBuilder, bool condition, object value)
        {
        if (condition)
            {
            stringBuilder.Append(value);
            }
        return stringBuilder;
        }

    public static StringBuilder AppendLineIf(this StringBuilder stringBuilder, bool condition, object value)
        {
        if (condition)
            {
            stringBuilder.AppendLine(value);
            }
        return stringBuilder;
        }

    public static StringBuilder AppendLine(this StringBuilder stringBuilder, object value)
        => stringBuilder.Append(value).AppendLine();

    public static StringBuilder AppendKeyValueString<TKey, TValue>(this StringBuilder stringBuilder, IEnumerable<KeyValuePair<TKey, TValue>> source, string keyValueSeparator = " = ", string entrySeparator = ", ")
        //=> stringBuilder.AppendDelimitedStrings(source.Select(pair => String.Concat(pair.Key, keyValueSeparator, pair.Value)), entrySeparator);
        { 
        bool isFirst = true;

        foreach (KeyValuePair<TKey, TValue> pair in source)
            {
            if (isFirst)
                {
                isFirst = false;
                }
            else
                {
                stringBuilder.Append(entrySeparator);
                }

            stringBuilder.Append(pair.Key).Append(keyValueSeparator).Append(pair.Value);
            }

        return stringBuilder;
        }

    /// <summary>
    /// Agrega una lista de strings delimitando los items con un separador.
    /// </summary>
    /// <param name="source">IEnumerable</param>
    /// <param name="separator">Separador de los items de la lista (", ", "\r\n", etc).</param>
    /// <returns>StringBuilder</returns>
    public static StringBuilder AppendDelimitedStrings(this StringBuilder stringBuilder, IEnumerable<string> source, string separator = ", ")
        => stringBuilder.AppendStrings(source.Interleave(separator));
        //=> source.AggregateNext((current, next, strBuilder, isLast) => strBuilder.Append(current).AppendIf(!isLast, separator), stringBuilder);

    /// <summary>
    /// Agrega una lista de strings.
    /// </summary>
    /// <param name="source">IEnumerable</param>
    /// <returns>StringBuilder</returns>
    public static StringBuilder AppendStrings(this StringBuilder stringBuilder, IEnumerable<string> source)
        => source.Aggregate(stringBuilder, (strBuilder, current) => strBuilder.Append(current));

    /// <summary>
    /// Idem StringBuilder.Remove pero si startIndex es negativo se considera desde el final de la cadena,
    /// y remueve todos los caracteres desde la posición indicada hasta el final.
    /// </summary>
    /// <param name="stringBuilder"></param>
    /// <param name="startIndex">
    /// Indica donde comienza la remoción, si es mayor o igual a cero, desde el inicio (base cero), sino, desde el final.
    /// </param>
    public static StringBuilder Remove(this StringBuilder stringBuilder, int startIndex)
        => StringBuilderExtensions.RemoveLast(stringBuilder, startIndex, startIndex < 0 ? -startIndex : stringBuilder.Length - startIndex);

    /// <summary>
    /// Idem StringBuilder.Remove pero si startIndex es negativo se considera desde el final de la cadena.
    /// </summary>
    /// <param name="stringBuilder"></param>
    /// <param name="startIndex">
    /// Indica donde comienza la remoción, si es mayor o igual a cero, desde el inicio (base cero), sino, desde el final.
    /// </param>
    /// <param name="length">Cantidad de caracteres a remover.</param>
    public static StringBuilder RemoveLast(this StringBuilder stringBuilder, int startIndex, int length)
        => stringBuilder.Length == 0 ? stringBuilder : stringBuilder.Remove(startIndex < 0 ? stringBuilder.Length + startIndex : startIndex, length);
    }
