using CommunityToolkit.HighPerformance;
using CoreClassLibrary.FileSystem;
using CoreClassLibrary.Streams;
using System.IO.Compression;

namespace CoreClassLibrary.Extensions;

public static class ZipExtensions
    {
    /// <summary>
    /// Comprime el archivo en un archivo del mismo nombre mas la extension ".gz"
    /// </summary>
    /// <param name="dataFilename">Archivo a comprimir</param>
    public static Task ZipAsync(string dataFilename, int bufferSize = 81920, CancellationToken cancellationToken = default) 
        => ZipExtensions.ZipAsync(dataFilename, String.Concat(dataFilename, ".gz"), bufferSize, cancellationToken);

    /// <summary>
    /// Comprime un archivo y lo guarda en otro.
    /// </summary>
    /// <param name="dataFilename">Archivo a comprimir</param>
    /// <param name="gzFilename">Archivo comprimido</param>
    public static async Task ZipAsync(string dataFilename, string gzFilename, int bufferSize = 81920, CancellationToken cancellationToken = default)
        {
        await using FileStream dataFileStream = new FileStream(dataFilename, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
        await using FileStream gzFileStream = new FileStream(gzFilename, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
        await using GZipStream gZipStream = new GZipStream(gzFileStream, CompressionMode.Compress);

        await dataFileStream.CopyToAsync(gZipStream, cancellationToken);
        }

    public static async Task ZipAsync(ReadOnlyMemory<byte> dataBuffer, Memory<byte> zipBuffer, CancellationToken cancellationToken = default)
        {
        await using GZipStream gZipStream = new GZipStream(zipBuffer.AsStream(), CompressionMode.Compress);

        await gZipStream.WriteAsync(dataBuffer, cancellationToken);
        await gZipStream.FlushAsync(cancellationToken);
        }

    /// <summary>
    /// Descomprime el archivo con extension ".gz" en un archivo del mismo nombre sin la extension ".gz"
    /// </summary>
    /// <param name="gzFilename">Archivo comprimido</param>
    public static Task UnzipAsync(string gzFilename, int bufferSize = 81920, CancellationToken cancellationToken = default) 
        => ZipExtensions.UnzipAsync(gzFilename, Path.Combine(Path.GetDirectoryName(gzFilename) ?? "", Path.GetFileNameWithoutExtension(gzFilename)), bufferSize, cancellationToken);

    /// <summary>
    /// Descomprime un archivo y lo guarda en otro.
    /// </summary>
    /// <param name="gzFilename">Archivo a descomprimir</param>
    /// <param name="dataFilename">Archivo descomprimido</param>
    public static async Task UnzipAsync(string gzFilename, string dataFilename, int bufferSize = 81920, CancellationToken cancellationToken = default)
        {
        await using FileStream gzFileStream = new FileStream(gzFilename, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
        await using GZipStream gZipStream = new GZipStream(gzFileStream, CompressionMode.Decompress);
        await using FileStream dataFileStream = new FileStream(dataFilename, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);

        await gZipStream.CopyToAsync(dataFileStream, cancellationToken);
        }

    public static async Task<int> UnzipAsync(ReadOnlyMemory<byte> zipBuffer, Memory<byte> dataBuffer, CancellationToken cancellationToken = default)
        {
        await using GZipStream gZipStream = new GZipStream(zipBuffer.AsStream(), CompressionMode.Decompress);

        int read = await gZipStream.ReadAsync(dataBuffer, cancellationToken);

        await gZipStream.FlushAsync(cancellationToken);

        return read;
        }

    }
