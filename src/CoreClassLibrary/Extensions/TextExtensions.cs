using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using System.Globalization;
using System.Text;

namespace CoreClassLibrary.Extensions;

public static class TextExtensions
    {
    public static string WriteInString(Action<TextWriter> writer)
        { 
        using PooledStringBuilder pooledStringBuilder = new PooledStringBuilder();
        TextExtensions.WriteInStringBuilder(writer, pooledStringBuilder.StringBuilder);
        return pooledStringBuilder.StringBuilder.ToString();
        } 
    
    public static void WriteInStringBuilder(Action<TextWriter> writer, StringBuilder stringBuilder)
        { 
        using StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture);
        writer(stringWriter);
        } 

    /// <summary>
    /// Enumerates all lines in a string.
    /// </summary>
    /// <param name="s">The string whose lines are to be enumerated.</param>
    /// <returns>An enumerable sequence of lines in this string.</returns>
    /// <see cref="https://github.com/thomaslevesque/NString/blob/master/src/NString/StringExtensions.cs"/>
    public static IEnumerable<string> ReadLines(this TextReader reader)
        {
        string? line;
        while ((line = reader.ReadLine()) != null)
            {
            yield return line;
            }
        }

    public static void RepeatWrite(this TextWriter writer, string value, int count)
        {
        for (int i = 0; i < count; i++)
            {
            writer.Write(value);
            }
        }

    public static void RepeatWrite(this TextWriter writer, char value, int count)
        {
        for (int i = 0; i < count; i++)
            {
            writer.Write(value);
            }
        }

    public static void WriteIf(this TextWriter writer, bool condition, Func<object> factory)
        {
        if (condition)
            {
            writer.Write(factory());
            }
        }

    public static void WriteLineIf(this TextWriter writer, bool condition, Func<object> factory)
        {
        if (condition)
            {
            writer.WriteLine(factory());
            }
        }

    public static void WriteIf(this TextWriter writer, bool condition, object value)
        {
        if (condition)
            {
            writer.Write(value);
            }
        }

    public static void WriteLineIf(this TextWriter writer, bool condition, object value)
        {
        if (condition)
            {
            writer.WriteLine(value);
            }
        }

    public static void WriteLine(this TextWriter writer, object value)
        {
        writer.Write(value);
        writer.WriteLine();
        }

    public static void WriteSerialized<T>(this TextWriter writer, Func<T, string> serializer, T state)
        {
        writer.WriteLine(typeof(T).FullName);
        writer.WriteLine(serializer.Invoke(state));
        }

    public static void TryWriteSerialized<T>(this TextWriter writer, Func<T, string> serializer, T state)
        {
        if (state != null)
            {
            writer.WriteSerialized(serializer, state);
            }
        }

    public static bool TryWriteSerialized<TImpl, TBase>(this TextWriter writer, Func<TImpl, string> serializer, TBase state) where TImpl : class, TBase
        {
        if (state is TImpl impl)
            {
            writer.WriteSerialized(serializer, impl);

            return true;
            }

        return false;
        }

    /// <summary>
    /// Agrega una lista de strings delimitando los items con un separador.
    /// </summary>
    /// <param name="source">IEnumerable</param>
    /// <param name="separator">Separador de los items de la lista (", ", "\r\n", etc).</param>
    /// <returns>TextWriter</returns>
    public static void WriteDelimitedStrings(this TextWriter writer, IEnumerable<string> source, string separator = ", ")
        => writer.WriteStrings(source.Interleave(separator));
    //=> source.AggregateNext((current, next, writer, isLast) => writer.Write(current).WriteIf(!isLast, separator), writer);

    /// <summary>
    /// Agrega una lista de strings.
    /// </summary>
    /// <param name="source">IEnumerable</param>
    /// <returns>TextWriter</returns>
    public static TextWriter WriteStrings(this TextWriter writer, IEnumerable<string> source)
        => source.Aggregate(writer, (w, current) => { w.Write(current); return w; });

    /// <summary>
    /// Lee la siguiente palabra.
    /// </summary>
    /// <see cref="https://stackoverflow.com/a/865679/5301336"/>
    public static string? ReadWord(this TextReader reader)
        {
        int c;

        // Ignore any trailing whitespace from previous reads
        while ((c = reader.Read()) != -1)
            {
            if (!char.IsWhiteSpace((char)c))
                {
                break;
                }
            }

        // Finished?
        if (c == -1)
            {
            return null;
            }

        using PooledStringBuilder builder = new PooledStringBuilder();
        builder.StringBuilder.Append((char)c);
        while ((c = reader.Read()) != -1)
            {
            if (char.IsWhiteSpace((char)c))
                {
                break;
                }
            builder.StringBuilder.Append((char)c);
            }
        return builder.StringBuilder.ToString();
        }

    public static string ReadUntil(this TextReader reader, char current)
        => reader.ReadUntil(current.Equals);

    public static string ReadUntil(this TextReader reader, Func<char, bool> predicate)
        => new string(reader.TakeUntil(predicate).ToArray());

    public static IEnumerable<char> TakeUntil(this TextReader reader, char current)
        => reader.TakeUntil(current.Equals);

    public static IEnumerable<char> TakeUntil(this TextReader reader, Func<char, bool> predicate)
        {
        int c;

        while ((c = reader.Read()) != -1)
            {
            if (predicate((char)c))
                {
                break;
                }
            yield return (char)c;
            }
        }


    public static char Readchar(this TextReader reader)
        {
        int c = reader.Read();
        return c != -1 ? (char)c : '\0';
        }

    }
