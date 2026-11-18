namespace CoreClassLibrary.Streams;

public class LimitedBufferStream(int maxBytes = 1024 * 1024) : Stream
    {
    private readonly MemoryStream inner = new MemoryStream();

    public bool LimitExceeded { get; private set; }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
        this.EnsureLimitExceeded(buffer.Length);
        await this.inner.WriteAsync(buffer, cancellationToken);
        }

    public override void Write(byte[] buffer, int offset, int count)
        {
        this.EnsureLimitExceeded(count);
        inner.Write(buffer, offset, count);
        }

    private void EnsureLimitExceeded(int count)
        {
        if (this.inner.Length + count > maxBytes)
            {
            this.LimitExceeded = true;
            throw new InvalidOperationException($"Response buffering limit {maxBytes} bytes exceeded");
            }
        }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        => this.inner.CopyToAsync(destination, bufferSize, cancellationToken);

    public override bool CanRead => false;
    public override bool CanSeek => true;
    public override bool CanWrite => true;
    public override long Length => this.inner.Length;
    public override long Position { get => this.inner.Position; set => this.inner.Position = value; }

    public override void Flush() { }
    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => this.inner.Seek(offset, origin);
    public override void SetLength(long value) => this.inner.SetLength(value);

    protected override void Dispose(bool disposing)
        {
        if (disposing)
            {
            inner.Dispose();
            }
        }
    }
