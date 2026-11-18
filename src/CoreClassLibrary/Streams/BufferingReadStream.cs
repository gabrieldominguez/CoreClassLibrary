namespace CoreClassLibrary.Streams;

/// <summary>
/// A Stream that wraps another stream and enables rewinding by buffering the content as it is read.
/// </summary>
/// <remarks>
/// Al escribir en esta clase se escribe solamente el buffer, el stream pasado en el constructor se considera de solo lectura.
/// </remarks>
/// <see cref="https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNetCore.WebUtilities/FileBufferingReadStream.cs"/>
/// <see cref="https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/io/memorystream.cs"/>
/// <see cref="https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/MemoryStream.cs"/>
public class BufferingReadStream(Stream innerStream) : MemoryStream
    {
    private bool completelyBuffered = false;

    public override int Read(byte[] buffer, int offset, int count)
        {
        if (this.Position < this.Length || this.completelyBuffered)
            {
            return base.Read(buffer, offset, count);
            }

        int read = innerStream.Read(buffer, offset, count);

        if (read > 0)
            {
            base.Write(buffer, offset, read);
            }
        else
            {
            this.completelyBuffered = true;
            }

        return read;
        }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
        if (this.Position < this.Length || this.completelyBuffered)
            {
            return await base.ReadAsync(buffer, offset, count, cancellationToken);
            }

        int read = await innerStream.ReadAsync(buffer, offset, count, cancellationToken);

        if (read > 0)
            {
            await base.WriteAsync(buffer, offset, read, cancellationToken);
            }
        else
            {
            this.completelyBuffered = true;
            }

        return read;
        }
    }
