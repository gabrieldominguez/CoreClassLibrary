using Xunit;
using CoreClassLibrary.Extensions;
using CoreClassLibrary.Exceptions.Domain;

namespace CoreClassLibrary.Tests;

public static class TestExtensions
    {
    public static void AssertAndWrite<T>(this ITestOutputHelper output, T? expected, T? actual)
        {
        Assert.Equal(expected, actual);

        output.WriteLine(actual?.ToString() ?? "null");
        }

    public static void Equal<T>(this ITestOutputHelper output, T? expected, T? actual)
        {
        output.WriteLine("expected:");
        output.WriteString(expected);
        output.WriteLine("");
        output.WriteLine("actual:");
        output.WriteString(actual);

        Assert.Equal(expected, actual);
        }

    public static void WriteString<T>(this ITestOutputHelper output, T? value)
        {
        string valueString = value?.ToString() ?? "null";

        output.WriteLine("Length: {0}", valueString.Length);
        output.WriteLine(valueString);
        output.WriteLine(valueString.ToDelimitedString());
        output.WriteLine(valueString.Select(c => char.GetNumericValue(c).ToString()).ToDelimitedString());
        output.WriteLine(valueString.Select(c => (byte)c).ToDelimitedString());
        //output.WriteLine(valueString.Select(c => ((byte)c).ToHEXString()).ToDelimitedString(""));
        }

    public static void WriteEnumerable<T>(this ITestOutputHelper output, IEnumerable<T> value)
        => output.WriteLine(String.Concat("[ ", value.ToDelimitedString(), " ]"));        

    public static void WriteLine(this ITestOutputHelper output)
        => output.WriteLine("");

    public static void Write(this ITestOutputHelper output, Exception ex)
        {
        output.WriteLine(ex.Message);
        output.WriteLine(ex.ToString());
        }

    public static void Write(this ITestOutputHelper output, DomainException ex)
        {
        output.WriteLine(ex.Message);
        output.WriteLine(ex.ToString());
        output.WriteLine();
        output.WriteLine("ValidationResults:");
        output.WriteEnumerable(ex.ValidationResults.Select(result => $"({result.MemberNames.First()}: \"{result.ErrorMessage}\")"));
        output.WriteLine("ValidationErrors:");
        output.WriteEnumerable(ex.GetValidationErrors().Select(error => $"({error.Key}: ({error.Value.ToDelimitedString()}))"));
        }

    public static IEnumerable<TSource> WriteLine<TSource>(this IEnumerable<TSource> source, ITestOutputHelper output)
        {
        foreach (TSource item in source)
            {
            output.WriteLine(item?.ToString() ?? "");
            yield return item;
            }
        }
    }
