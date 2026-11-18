using System.Buffers;

namespace CoreClassLibrary.Objects.Pooling;

/// <summary>
/// RAII Memory
/// </summary>
/// <remarks>
/// 
///     UTILIZAR SIEMPRE EL CONSTRUCTOR CON PARAMETRO.
///     public PooledMemory(int length)
/// 
///     Es un struct para no generar una alocacion que es lo que trata de evitar el patron pooling.
///     Es un struct inmutable por lo que es segura de usar:
///     https://stackoverflow.com/questions/7914423/struct-and-idisposable
///     https://stackoverflow.com/questions/441309/why-are-mutable-structs-evil
/// 
/// </remarks>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/communitytoolkit/high-performance/memoryowner"/>
/// <see cref="https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.HighPerformance/Buffers/MemoryOwner%7BT%7D.cs"/>
public readonly record struct PooledMemory<T> : IDisposable
    {
    /// <summary>
    /// Memory solicitada: tiene exactamente los elementos solicitados.
    /// </summary>
    public readonly Memory<T> Memory;
    
    private readonly IMemoryOwner<T> memoryOwner;

    public PooledMemory(int length)
        {
        this.Memory = (this.memoryOwner = MemoryPool<T>.Shared.Rent(length)).Memory[..length];
        }

    public void Dispose() 
        => this.memoryOwner.Dispose();
    }
