namespace CoreClassLibrary.Mathematics.IdGenerators;

/// <summary>
/// Generates an aspnetcore TraceIdentifier
/// </summary>
/// <remarks>
/// The id is 22 characters length
/// </remarks>
/// <example>0HMMJO8IKDICV:00000001</example>
/// <see cref="https://github.com/aspnet/KestrelHttpServer/blob/0aff4a0440c2f393c0b98e9046a8e66e30a56cb0/src/Kestrel.Core/Internal/Http/Http1Connection.cs#L446"/>
/// <see cref="https://github.com/aspnet/KestrelHttpServer/blob/a48222378b8249a26b093b5b835001c7c7b45815/src/Kestrel.Core/Internal/Infrastructure/CorrelationIdGenerator.cs"/>
/// <see cref="http://www.nimaara.com/2018/10/10/generating-ids-in-csharp"/>
/// <see cref="https://stackoverflow.com/questions/50689315/how-is-httpcontext-traceidentifier-generated-in-net-core"/>
/// <see cref="Base32IdGenerator"/>
/// <see cref="Hex8IdGenerator"/>
public class TraceIdentifierGenerator(long prefix, IIdGenerator<int> intGenerator) : IIdGenerator<string>
    {
    private readonly string prefix = String.Concat(Base32IdGenerator.EncodeId(prefix), ":");

    public TraceIdentifierGenerator(IIdGenerator<int> intGenerator) : this(DateTime.UtcNow.Ticks, intGenerator) { }

    public string NextId()
        => String.Concat(this.prefix, Hex8IdGenerator.EncodeId(intGenerator.NextId()));
    }
