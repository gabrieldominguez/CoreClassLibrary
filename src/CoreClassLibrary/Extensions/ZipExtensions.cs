using CoreClassLibrary.Streams;
using System.IO.Compression;

namespace CoreClassLibrary.Extensions;

public static class ZipExtensions
    {
    /// <summary>
    /// Comprime y retorna un byte[].
    /// </summary>
    [Obsolete("Rewrite without byte[]")]
    public static async Task<byte[]> GZipCompressAsync(this byte[] dataBuffer, CancellationToken cancellationToken = default)
        {
        await using MemoryStream gzipMemoryStream = MemoryStreamManager.Instance.GetStream();
        await using GZipStream gZipStream = new GZipStream(gzipMemoryStream, CompressionMode.Compress);

        await gZipStream.WriteAsync(dataBuffer, cancellationToken);
        await gZipStream.FlushAsync(cancellationToken);

        return gzipMemoryStream.ToArray();
        }

    /// <summary>
    /// Descomprime y retorna un byte[].
    /// </summary>
    [Obsolete("Rewrite without byte[]")]
    public static Task<byte[]> GZipDecompressAsync(this byte[] gzipBuffer, int bufferSize = StreamExtensions.DefaultReadBufferSize, CancellationToken cancellationToken = default)
        {
        using MemoryStream gzipMemoryStream = new MemoryStream(gzipBuffer);
        using GZipStream gZipStream = new GZipStream(gzipMemoryStream, CompressionMode.Decompress);
        return gZipStream.ReadAllBytesAsync(bufferSize, cancellationToken);
        }

    public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, int bufferSize = StreamExtensions.DefaultReadBufferSize, CancellationToken cancellationToken = default)
        {
        await using MemoryStream memoryStream = MemoryStreamManager.Instance.GetStream();
        await stream.CopyToAsync(memoryStream, bufferSize, cancellationToken);
        return memoryStream.ToArray();
        }     }
