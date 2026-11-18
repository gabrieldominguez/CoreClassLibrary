using CommunityToolkit.HighPerformance;
using System.IO.Compression;
using CoreClassLibrary.Objects.Pooling;

namespace CoreClassLibrary.Streams;

/// <summary>
/// Stream Extensions Methods
/// </summary>
/// <remarks>
/// To discard a stream, use Stream.Null (NullStream) instead of null.  
/// This is a stream with no backing store that can be written to and read from, 
/// but the data is discarded and the stream always appears empty.
/// </remarks>
/// <see cref="https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/IO/Stream.cs"/>
/// <see cref="https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/IO/StreamReader.cs"/>
/// <see cref="https://github.com/dotnet/corefx/blob/master/src/System.Runtime.Extensions/src/System/IO/StreamWriter.cs"/>
public static class StreamExtensions
    {
    // We pick a value that is the largest multiple of 4096 that is still smaller than the large object heap threshold (85K).
    // The CopyTo/CopyToAsync buffer is short-lived and is likely to be collected at Gen0, and it offers a significant
    // improvement in Copy performance.
    // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/IO/Stream.cs#L40
    //public const int DefaultBufferSize = 81920;

    // Using a 1K byte buffer and a 4K FileStream buffer works out pretty well perf-wise.  On even a 40 MB text file, any perf loss by using a 4K
    // buffer is negated by the win of allocating a smaller byte[], which 
    // saves construction time.  This does break adaptive buffering,
    // but this is slightly faster.
    // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/IO/StreamReader.cs#L36
    public const int DefaultReadBufferSize = 1024;
    public const int DefaultFileReadBufferSize = 4096;

    // For UTF-8, the values of 1K for the default buffer size and 4K for the
    // file stream buffer size are reasonable & give very reasonable
    // performance for in terms of construction time for the StreamWriter and
    // write perf.  Note that for UTF-8, we end up allocating a 4K byte buffer,
    // which means we take advantage of adaptive buffering code.
    // The performance using UnicodeEncoding is acceptable.  
    // https://github.com/dotnet/corefx/blob/master/src/System.Runtime.Extensions/src/System/IO/StreamWriter.cs#L22
    public const int DefaultWriteBufferSize = 1024;
    public const int DefaultFileWriteBufferSize = 4096;

    /// <summary>
    /// Try to set the position to the beginning of the stream, if the stream supports seeking.
    /// </summary>
    public static Stream TrySeekToBegin(this Stream stream, int position = 0)
        {
        if (stream.CanSeek)
            {
            stream.SeekToBegin(position);
            }
        return stream;
        }

    /// <summary>
    /// Set the position to the beginning of the stream.
    /// </summary>
    public static Stream SeekToBegin(this Stream stream, int position = 0)
        {
        //stream.Position = position;
        stream.Seek(position, SeekOrigin.Begin);
        return stream;
        }

    /// <summary>
    /// Escribe el string en el stream.
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/1879395/how-do-i-generate-a-stream-from-a-string"/>
    public static void WriteToStream(this string str, Stream stream)
        {
        StreamWriter writer = new StreamWriter(stream, leaveOpen: true);
        writer.Write(str);
        writer.Flush();
        //stream.Position = 0;
        }

    /// <summary>
    /// Escribe el string en el stream.
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/1879395/how-do-i-generate-a-stream-from-a-string"/>
    public static async Task WriteToStreamAsync(this string str, Stream stream)
        {
        StreamWriter writer = new StreamWriter(stream, leaveOpen: true);
        await writer.WriteAsync(str);
        await writer.FlushAsync();
        //stream.Position = 0;
        }

    /// <summary>
    /// Copy the stream to a string as an asynchronous operation.
    /// </summary>
    public static async Task<string> ReadAsStringAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
        StreamExtensions.TrySeekToBegin(stream);
        string strBody = await StreamExtensions.ReadToEndAsync(stream, cancellationToken);
        StreamExtensions.TrySeekToBegin(stream);
        return strBody;
        }

    /// <summary>
    /// Lee todos los caracteres desde la posición actual hasta el final de la secuencia.
    /// </summary>
    /// <param name="stream"></param>
    /// <remarks>
    /// Don't wrap the StreamReader creation in a using statement though or it will close the underlying stream at the conclusion of the using block
    /// and code later wont be able to read the body.
    /// StreamReader.Dispose() solo libera el stream subyacente, asi que es correcto ignorarlo cuando el stream se recibe por parametro.
    /// https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/IO/StreamReader.cs#L234
    ///
    /// Similar a Encoding.UTF8.GetString(stream.ReadAllBytes()));
    /// </remarks>
    /// <returns>
    /// Resto de la secuencia, como una cadena, desde la posición actual hasta el final.
    /// Si la posición actual se encuentra al final de la secuencia o si esta no puede ser leída, devuelve una cadena vacía ("").
    /// </returns>
    /// <see cref="https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/IO/StreamReader.cs"/>
    public static Task<string> ReadToEndAsync(this Stream stream, CancellationToken cancellationToken = default)
        => stream.CanRead ? new StreamReader(stream).ReadToEndAsync(cancellationToken) : Task.FromResult<string>("");

    public static async Task<TStream> CopyAllBytesToStreamAsync<TStream>(this Stream source, int bufferSize = StreamExtensions.DefaultWriteBufferSize, CancellationToken cancellationToken = default) where TStream : Stream, new()
        {
        StreamExtensions.TrySeekToBegin(source);

        TStream destination = await StreamExtensions.CopyToStreamAsync<TStream>(source, bufferSize, cancellationToken);

        StreamExtensions.TrySeekToBegin(source);
        StreamExtensions.TrySeekToBegin(destination);

        return destination;
        }

    public static async Task<TStream> CopyToStreamAsync<TStream>(this Stream source, int bufferSize = StreamExtensions.DefaultWriteBufferSize, CancellationToken cancellationToken = default) where TStream : Stream, new()
        {
        TStream stream = new TStream();
        await source.CopyToAsync(stream, bufferSize, cancellationToken);
        return stream;
        }

    /// <summary>
    /// Enumerate all bytes from a stream.
    /// </summary>
    public static IEnumerable<byte> EnumerateAllBytes(this Stream source, int bufferSize = StreamExtensions.DefaultReadBufferSize)
        {
        using PooledMemory<byte> buffer = new PooledMemory<byte>(bufferSize);
        int bytesRead;
        while ((bytesRead = source.Read(buffer.Memory.Span)) > 0)
            {
            for (int i = 0; i < bytesRead; i++)
                {
                yield return buffer.Memory.Span[i];
                }
            }
        }

    public static async Task ZipAsync(ReadOnlyMemory<byte> dataBuffer, Memory<byte> zipBuffer, CancellationToken cancellationToken = default)
        {
        await using GZipStream gZipStream = new GZipStream(zipBuffer.AsStream(), CompressionMode.Compress);

        await gZipStream.WriteAsync(dataBuffer, cancellationToken);
        await gZipStream.FlushAsync(cancellationToken);
        }

    public static async Task<int> UnzipAsync(ReadOnlyMemory<byte> zipBuffer, Memory<byte> dataBuffer, CancellationToken cancellationToken = default)
        {
        await using GZipStream gZipStream = new GZipStream(zipBuffer.AsStream(), CompressionMode.Decompress);

        int read = await gZipStream.ReadAsync(dataBuffer, cancellationToken);

        await gZipStream.FlushAsync(cancellationToken);

        return read;
        }
    }
