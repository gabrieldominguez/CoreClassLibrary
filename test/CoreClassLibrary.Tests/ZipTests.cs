#pragma warning disable IDE0305 // Simplify collection initialization

using CoreClassLibrary.Extensions;
using CoreClassLibrary.Objects.Pooling;
using Xunit;

namespace CoreClassLibrary.Tests;

public class ZipTests(ITestOutputHelper output)
    {
    [Fact]
    public async Task ZipAsyncFiles()
        {
        string dataFilename = Path.GetTempFileName();
        string gzFilename = Path.GetTempFileName();
        string unzippedFilename = Path.GetTempFileName();
        int bufferSize = 81920;
        byte[] data = Enumerable.Range(0, 1024).Select(index => (byte)(index % 256)).ToArray();

        try
            {
            await File.WriteAllBytesAsync(dataFilename, data, TestContext.Current.CancellationToken);

            await ZipExtensions.ZipAsync(dataFilename, gzFilename, bufferSize, TestContext.Current.CancellationToken);
            await ZipExtensions.UnzipAsync(gzFilename, unzippedFilename, bufferSize, TestContext.Current.CancellationToken);

            Assert.Equal(data, await File.ReadAllBytesAsync(unzippedFilename, TestContext.Current.CancellationToken));
            }
        finally
            {
            File.Delete(dataFilename);
            File.Delete(gzFilename);
            File.Delete(unzippedFilename);
            }
        }

    [Fact]
    // https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream.-ctor?view=net-9.0&devlangs=csharp&f1url=%3FappId%3DDev17IDEF1%26l%3DEN-US%26k%3Dk(System.IO.Compression.GZipStream.%2523ctor)%3Bk(SolutionItemsProject)%3Bk(DevLang-csharp)%26rd%3Dtrue
    public async Task Zip()
        {
        const int bufferLength = 50;
        const int dataLength = 20;

        using PooledMemory<byte> data = new PooledMemory<byte>(bufferLength);
        using PooledMemory<byte> ziped = new PooledMemory<byte>(bufferLength);
        using PooledMemory<byte> unziped = new PooledMemory<byte>(bufferLength);

        data.Memory.Span[..dataLength].Fill(1);

        output.WriteLine("data");
        output.WriteLine(data.Memory.ReadAsEnumerable().ToByteArrayLiteral());

        await ZipExtensions.ZipAsync(data.Memory[..dataLength], ziped.Memory, TestContext.Current.CancellationToken);

        output.WriteLine("");
        output.WriteLine("zipped");
        output.WriteLine(ziped.Memory.ReadAsEnumerable().ToByteArrayLiteral());

        int read = await ZipExtensions.UnzipAsync(ziped.Memory, unziped.Memory, TestContext.Current.CancellationToken);

        output.WriteLine("");
        output.WriteLine("unzipped");
        output.WriteLine(unziped.Memory.ReadAsEnumerable().ToByteArrayLiteral());

        Assert.Equal(dataLength, read);
        Assert.Equal(data.Memory[..dataLength], unziped.Memory[..read]);
        }
    
    [Theory]
    [InlineData("Hola mundo")]
    public async Task ZipString(string text)
        {
        const int bufferLength = 500;

        using PooledMemory<byte> data = new PooledMemory<byte>(bufferLength);
        using PooledMemory<byte> ziped = new PooledMemory<byte>(bufferLength);
        using PooledMemory<byte> unziped = new PooledMemory<byte>(bufferLength);

        int length = StringExtensions.UTF8Encoding.GetBytes(text.RepeatAppend(10), data.Memory.Span);

        output.WriteLine(Convert.ToBase64String(data.Memory.Span));

        await ZipExtensions.ZipAsync(data.Memory[..length], ziped.Memory, TestContext.Current.CancellationToken);

        output.WriteLine(Convert.ToBase64String(ziped.Memory.Span));

        await ZipExtensions.UnzipAsync(ziped.Memory, unziped.Memory, TestContext.Current.CancellationToken);

        Assert.NotEqual(data.Memory, ziped.Memory);
        Assert.Equal(data.Memory, unziped.Memory);
        }        
    }
