using System.Buffers;

namespace CoreClassLibrary.Objects.Pooling;

/// <summary>
/// RAII array
/// </summary>
/// <remarks>
///     Usar PooledMemory siempre que sea posible:
///     PooledArray puede inducir a error ya que el arreglo obtenido es usualmente mayor al pedido.
///     El Memory obtenido con PooledMemory es exactamente de la longitud pedida.
/// 
///     Utilizar siempre este constructor: PooledArray(int length) (ya que los struct permiten crearse sin constructor: new PooledArray<int>()
///     
///     Es un struct para no generar una alocacion que es lo que trata de evitar el patron pooling.
///     Es un struct inmutable por lo que es segura de usar:
///     https://stackoverflow.com/questions/7914423/struct-and-idisposable
///     https://stackoverflow.com/questions/441309/why-are-mutable-structs-evil
/// 
///     El arreglo pedido puede tener mas elementos que la cantidad solicitada (y esto es reflejado en la propiedad Length del mismo).
///     Los elementos pueden estar inicializados o no, pero no deben tenerse en cuenta.
///     Por lo tanto, este arreglo no puede recorrerse totalmente, como por ej. usando un foreach.
///     La propiedad PooledArray<T>.Memory.Span.Length almacena la cantidad solicitada, por lo que debe usarse en lugar de Array.Length.
/// 
/// </remarks>    
/// <code>
///     Sin esta clase:
///     byte[] buffer = this.arrayPool.Rent(8);
///     try
///         {
///         this.randomNumberGenerator.GetBytes(buffer);
///         return BitConverter.ToDouble(buffer, 0);
///         }
///     finally
///         {
///         this.arrayPool.Return(buffer);
///         }
///     
///     Con esta clase:
///     using (PooledArray<byte> pooledArray = new PooledArray<byte>(8, this.arrayPool))
///         {
///         this.randomNumberGenerator.GetBytes(pooledArray.Array);
///         return BitConverter.ToDouble(pooledArray.Array, 0);
///         }
/// </code>
/// <see cref="PooledMemory"/>
public readonly record struct PooledArray<T> : IDisposable
    {
    /// <summary>
    /// Arreglo solicitado: puede tener más elementos que los solicitados.
    /// </summary>
    /// <remarks>Puede utilizarse Memory.Length para obtener la cantidad solicitada.</remarks>
    public readonly T[] Array;

    /// <summary>
    /// Arreglo solicitado: tiene exactamente los elementos solicitados.
    /// </summary>
    /// <remarks>
    /// Considerar el uso de IMemoryOwner en lugar de PooledArray
    /// </remarks>
    public readonly Memory<T> Memory;

    public PooledArray(int length)
        {
        this.Memory = (this.Array = ArrayPool<T>.Shared.Rent(length)).AsMemory(0, length);
        }

    public void Dispose() 
        => ArrayPool<T>.Shared.Return(this.Array);
    }
