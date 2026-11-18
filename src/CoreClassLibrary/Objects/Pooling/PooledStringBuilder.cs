using System.Text;

namespace CoreClassLibrary.Objects.Pooling;

/// <summary>
/// RAII StringBuilder
/// </summary>
/// <see cref="ValueStringBuilder"/>
/// <see cref="StringBuilderCache"/>
/// <see cref="https://andrewlock.net/series/a-deep-dive-on-stringbuilder"/>
/// <see cref="https://www.stevejgordon.co.uk/how-does-the-stringbuilder-work-in-dotnet-part-1-why-do-we-need-a-stringbuilder-and-when-should-we-use-one"/>
/// <see cref="https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/Text/StringBuilderCache.cs"/>
/// <see cref="https://github.com/dotnet/runtime/blob/0605bb3aba533702f234c907906ef076a97131fe/src/libraries/System.Private.CoreLib/src/System/Text/StringBuilder.cs"/>
public class PooledStringBuilder(int capacity = StringBuilderCache.DefaultCapacity) : IDisposable
    {
    public readonly StringBuilder StringBuilder = StringBuilderCache.Acquire(capacity);

    public void Dispose()
        {
        StringBuilderCache.Release(this.StringBuilder);
        GC.SuppressFinalize(this);
        }

    public override string ToString() 
        => this.StringBuilder.ToString();

    public static string ToString(Action<StringBuilder> configure)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        configure(builder.StringBuilder);
        return builder.StringBuilder.ToString();
        }

    public static string ToString<TContext>(TContext context, Action<TContext, StringBuilder> configure)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        configure(context, builder.StringBuilder);
        return builder.StringBuilder.ToString();
        }

    public static async Task<string> ToStringAsync(Func<StringBuilder, Task> configureAsync)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        await configureAsync(builder.StringBuilder);
        return builder.StringBuilder.ToString();
        }

    public static async Task<string> ToStringAsync(Func<StringBuilder, CancellationToken, Task> configureAsync, CancellationToken cancellationToken = default)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        await configureAsync(builder.StringBuilder, cancellationToken);
        return builder.StringBuilder.ToString();
        }

    public static async Task<string> ToStringAsync<TContext>(TContext context, Func<TContext, StringBuilder, Task> configureAsync)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        await configureAsync(context, builder.StringBuilder);
        return builder.StringBuilder.ToString();
        }

    public static async Task<string> ToStringAsync<TContext>(TContext context, Func<TContext, StringBuilder, CancellationToken, Task> configureAsync, CancellationToken cancellationToken = default)
        {
        using PooledStringBuilder builder = new PooledStringBuilder();
        await configureAsync(context, builder.StringBuilder, cancellationToken);
        return builder.StringBuilder.ToString();
        }
    }