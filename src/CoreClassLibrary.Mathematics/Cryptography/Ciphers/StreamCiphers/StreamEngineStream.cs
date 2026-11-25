namespace CoreClassLibrary.Mathematics.Cryptography.Ciphers.StreamCiphers;

/// <summary>
/// Encapsula un stream como destino de operaciones de encriptacion y desencriptacion.
/// </summary>
public class StreamEngineStream : Stream
    {
    private readonly Stream stream;
    private readonly IStreamEngine streamEngine;

    public StreamEngineStream(Stream stream, IStreamEngine streamEngine)
        {
        this.stream = stream;
        (this.streamEngine = streamEngine).Initialize();
        }

    public override long Position { get => this.stream.Position; set => this.stream.Position = value; }
    public override bool CanSeek => this.stream.CanSeek;
    public override long Seek(long offset, SeekOrigin origin)
        => this.stream.Seek(offset, origin);

    public override long Length => this.stream.Length;
    public override void SetLength(long value)
        => this.stream.SetLength(value);

    public override bool CanRead => false;
    public override int Read(byte[] output, int outputOffset, int outputCount)
        => throw new InvalidOperationException("Stream does not support reading.");

    public override bool CanWrite => this.stream.CanWrite;

    public void Write(byte[] input)
        => this.Write(input, 0, input.Length);

    public void Write(byte[] input, int offset)
        => this.Write(input, offset, input.Length - offset);

    // Summary:
    //     Encrypt or decrypt a sequence of bytes and writes it to the current stream.
    //
    // Parameters:
    //   buffer:
    //     An array of bytes. This method encrypt or decrypt count bytes from buffer to the current stream.
    //
    //   offset:
    //     The zero-based byte offset in buffer at which to encrypting or decrypting copying bytes to the current stream.
    //
    //   count:
    //     The number of bytes to be written to the current stream.
    public override void Write(byte[] input, int offset, int count)
        {
        for (int i = 0; i < count; i++)
            {
            this.stream.WriteByte(this.streamEngine.ProcessByte(input[offset + i]));
            }
        }

    public override void Flush() 
        => this.stream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) 
        => this.stream.FlushAsync(cancellationToken);

    public override void Close() 
        => this.stream.Close();
    }
