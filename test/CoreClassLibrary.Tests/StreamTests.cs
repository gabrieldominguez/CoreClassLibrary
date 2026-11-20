using CoreClassLibrary.Streams;
using Xunit;

namespace CoreClassLibrary.Tests;

public class StreamTests
    {
    [Fact]
    public async Task LimitedBufferStream_WriteAndSeek_ReadPropertiesMatchData()
        {
        using LimitedBufferStream stream = new(10);
        await stream.WriteAsync(new byte[] { 1, 2, 3 }, TestContext.Current.CancellationToken);

        Assert.True(stream.CanWrite);
        Assert.False(stream.CanRead);
        Assert.True(stream.CanSeek);
        Assert.Equal(3, stream.Length);
        Assert.Equal(3, stream.Position);
        Assert.False(stream.LimitExceeded);

        stream.Position = 0;
        Assert.Equal(0, stream.Position);
        Assert.Equal(0, stream.Seek(0, SeekOrigin.Begin));
    }

    [Fact]
    public void LimitedBufferStream_ExceedingLimit_ThrowsAndMarksLimitExceeded()
        {
        using LimitedBufferStream stream = new(3);

        Assert.Throws<InvalidOperationException>(() => stream.Write(new byte[4], 0, 4));
        Assert.True(stream.LimitExceeded);
    }

    [Fact]
    public async Task LimitedBufferStream_AsyncWriteWithinLimit_Succeeds()
        {
        using LimitedBufferStream stream = new(4);

        await stream.WriteAsync(new byte[] { 1, 2, 3 }, TestContext.Current.CancellationToken);
        await stream.FlushAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, stream.Length);
    }

    [Fact]
    public void LimitedBufferStream_ReadAndUnsupportedOperations_Throw()
        {
        using LimitedBufferStream stream = new();

        Assert.Throws<NotSupportedException>(() => stream.Read([], 0, 0));
        Assert.Throws<NotSupportedException>(() => stream.ReadByte());
    }

    [Fact]
    public void LimitedBufferStream_SetLength_ChangesLength()
        {
        using LimitedBufferStream stream = new();
        stream.Write([1, 2, 3], 0, 3);

        stream.SetLength(2);

        Assert.Equal(2, stream.Length);
    }
}
