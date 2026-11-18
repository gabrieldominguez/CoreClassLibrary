namespace CoreClassLibrary.Streams;

/// <summary>
/// A Stream that wraps another stream and enables buffering the content as it is write.
/// </summary>
/// <remarks>
/// Al leer de esta clase se lee solamente del buffer, el stream pasado en el constructor se considera de solo escritura.
/// </remarks>
/// <see cref="https://stackoverflow.com/questions/42985032/not-disposing-stream-in-net-core"/>
/// <see cref="https://github.com/aspnet/BasicMiddleware/blob/master/src/Microsoft.AspNetCore.Buffering/BufferingWriteStream.cs"/>
/// <see cref="https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/io/memorystream.cs"/>
/// <see cref="https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/MemoryStream.cs"/>
public class BufferingWriteStream(Stream innerStream, bool flushInnerStream = true) : MemoryStream
    {
    public override void Flush()
        {
        if (this.Length > 0)
            {
            this.Seek(0, SeekOrigin.Begin);
            this.CopyTo(innerStream);
            this.Seek(0, SeekOrigin.Begin);
            this.SetLength(0);
            }

        if (flushInnerStream)
            {
            innerStream.Flush();
            }
        }

    public override async Task FlushAsync(CancellationToken cancellationToken)
        {
        if (this.Length > 0)
            {
            this.Seek(0, SeekOrigin.Begin);
            await this.CopyToAsync(innerStream, cancellationToken);
            this.Seek(0, SeekOrigin.Begin);
            this.SetLength(0);
            }

        if (flushInnerStream)
            {
            await innerStream.FlushAsync(cancellationToken);
            }        
        }
    }
