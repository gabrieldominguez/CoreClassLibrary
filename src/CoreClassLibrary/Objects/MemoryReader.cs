namespace CoreClassLibrary.Objects;

/// <summary>
/// Permite obtener items y subarrays secuencialmente.
/// </summary>
/// <see cref="StringParser"/>
/// <see cref="StringSegment"/>
/// <see cref="ArraySegment"/>
/// <see cref="Memory"/>
/// <see cref="Span"/>
/// <see cref="https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Memory.cs"/>
/// <see cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.server.kestrel.internal.system.span-1.toarray"/>
public class MemoryReader<T>(Memory<T> memory, int offset = 0)
    {
    public MemoryReader(T[] array, int offset = 0) : this(new Memory<T>(array), offset) { }

    public T GetNextItem()
        => memory.Span[offset++];

    public Memory<T> GetNextMemory(int length)
        {
        Memory<T> mem = memory.Slice(offset, length);

        offset += length;

        return mem;
        }
    }
