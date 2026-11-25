
using CoreClassLibrary.Extensions;
using Xunit;

namespace CoreClassLibrary.Tests.Processing;

public static class Shared
    {
    public static readonly IEnumerable<string> Source = StringExtensions.LowerCaseAlphaChars.Select(c => c.ToString());
    public static readonly IEnumerable<string> Result = StringExtensions.UpperCaseAlphaChars.Select(c => c.ToString());

    public static readonly IAsyncEnumerable<string> AsyncSource = Shared.Source.ToAsyncEnumerable();
    public static readonly IAsyncEnumerable<string> AsyncResult = Shared.Result.ToAsyncEnumerable();

    public static Task<string> ProcessAsync(string line, CancellationToken cancellationToken = default)
        => Task.Run(() => Shared.Process(line), cancellationToken);

    public static string Process(string line)
        => line.ToUpperInvariant();

    public static ValueTask TestValueAsync(string line, ITestOutputHelper output, CancellationToken cancellationToken = default)
        => new ValueTask(Shared.TestAsync(line, output, cancellationToken));

    public static Task TestAsync(string line, ITestOutputHelper output, CancellationToken cancellationToken = default)
        => Task.Run(() => Shared.Test(line, output), cancellationToken);

    public static void Test(string line, ITestOutputHelper output)
        { 
        string result = Shared.Process(line);
        output.WriteLine(result);
        Assert.Contains(result, Shared.Result);
        }
    }
