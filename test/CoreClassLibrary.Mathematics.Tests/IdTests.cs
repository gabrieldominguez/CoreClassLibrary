#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Mathematics.IdGenerators;
using CoreClassLibrary.Mathematics.Randomness;
using Xunit;

namespace CoreClassLibrary.Mathematics.Tests;

public class IdTests(ITestOutputHelper output)
    {
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Base32IdGenerator(long seed)
        {
        const int length = 13;

        string idSeed = new Base32IdGenerator(new LongIdGenerator(seed)).NextId();
        output.WriteLine(idSeed);
        Assert.Equal(length, idSeed.Length);

        string id = new Base32IdGenerator(new LongIdGenerator()).NextId();
        output.WriteLine(id);
        Assert.Equal(length, id.Length);

        Assert.NotEqual(idSeed, id);
        Assert.NotEqual(id, new Base32IdGenerator(new LongIdGenerator()).NextId());
        }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Base64IdGenerator(long seed)
        {
        const int length = 12;

        string idSeed = new Base64IdGenerator(new LongIdGenerator(seed)).NextId();
        output.WriteLine(idSeed);
        Assert.Equal(length, idSeed.Length);

        string id = new Base64IdGenerator(new LongIdGenerator()).NextId();
        output.WriteLine(id);
        Assert.Equal(length, id.Length);

        Assert.NotEqual(idSeed, id);
        Assert.NotEqual(id, new Base64IdGenerator(new LongIdGenerator()).NextId());
        }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void TraceIdentifierGenerator(int seed)
        {
        const int length = 22;

        string idSeed = new TraceIdentifierGenerator(new IntIdGenerator(seed)).NextId();
        output.WriteLine(idSeed);
        Assert.Equal(length, idSeed.Length);

        string id = new TraceIdentifierGenerator(new IntIdGenerator()).NextId();
        output.WriteLine(id);
        Assert.Equal(length, id.Length);

        Assert.NotEqual(idSeed, id);
        Assert.NotEqual(id, new TraceIdentifierGenerator(new IntIdGenerator()).NextId());
        }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Hex8IdGenerator(int seed)
        {
        const int length = 8;

        string idSeed = new Hex8IdGenerator(new IntIdGenerator(seed)).NextId();
        output.WriteLine(idSeed);
        Assert.Equal(length, idSeed.Length);

        string id = new Hex8IdGenerator(new IntIdGenerator()).NextId();
        output.WriteLine(id);
        Assert.Equal(length, id.Length);

        Assert.NotEqual(idSeed, id);
        Assert.NotEqual(id, new Hex8IdGenerator(new IntIdGenerator()).NextId());
        }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Hex16IdGenerator(long seed)
        {
        const int length = 16;

        string idSeed = new Hex16IdGenerator(new LongIdGenerator(seed)).NextId();
        output.WriteLine(idSeed);
        Assert.Equal(length, idSeed.Length);

        string id = new Hex16IdGenerator(new LongIdGenerator()).NextId();
        output.WriteLine(id);
        Assert.Equal(length, id.Length);

        Assert.NotEqual(idSeed, id);
        Assert.NotEqual(id, new Hex16IdGenerator(new LongIdGenerator()).NextId());
        }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void TestSequentialGuidIdGenerator(long seed)
        {
        Guid idSeed = new SequentialGuidIdGenerator(new LongIdGenerator(seed)).NextId();
        output.WriteLine(idSeed.ToString());

        Guid id = new SequentialGuidIdGenerator(new LongIdGenerator()).NextId();
        output.WriteLine(id.ToString());

        Assert.NotEqual(idSeed, id);
        Assert.NotEqual(id, new SequentialGuidIdGenerator(new LongIdGenerator()).NextId());
        }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void FormattedSequentialGuidIdGenerator(long seed)
        {
        const int length = 36;

        string idSeed = new FormattedGuidIdGenerator(new SequentialGuidIdGenerator(new LongIdGenerator(seed))).NextId();
        output.WriteLine(idSeed);
        Assert.Equal(length, idSeed.Length);

        string id = new FormattedGuidIdGenerator(new SequentialGuidIdGenerator(new LongIdGenerator())).NextId();
        output.WriteLine(id);
        Assert.Equal(length, id.Length);

        Assert.NotEqual(idSeed, id);
        Assert.NotEqual(id, new FormattedGuidIdGenerator(new SequentialGuidIdGenerator(new LongIdGenerator())).NextId());
        }

    [Fact]
    public void NextSequentialGuid()
        {
        Guid[] guids = EnumerableExtensions.Repeat(SequentialGuidIdGenerator.NewGuid, 20).ToArray();

        foreach (Guid guid in guids)
            {
            output.WriteLine(guid.ToString());
            }
        output.WriteLine("");
        foreach (Guid guid in guids)
            {
            output.WriteLine(guid.ToByteArray().ToHexString());
            }

        //Assert.True(EnumerableExtensions.IsOrdered(guids));
        }

    [Fact]
    public void GuidIdGenerator()
        {
        Guid id = new GuidIdGenerator().NextId();
        output.WriteLine(id.ToString());
        }
    
    [Fact]
    public void RandomTraceIdentifier()
        {
        string traceIdentifier = new TraceIdentifierGenerator(new RandomIdGenerator()).NextId();
        output.WriteLine(traceIdentifier);
        Assert.Equal(22, traceIdentifier.Length);
        }

    [Fact]
    public void DelegateIdGenerator()
        {
        output.WriteLine(new DelegateIdGenerator<int>(RandomWrapper.Shared.Next<int>).NextId().ToString());
        }
    
    }
