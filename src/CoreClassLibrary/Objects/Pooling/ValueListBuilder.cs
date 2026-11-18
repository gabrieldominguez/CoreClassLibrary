#pragma warning disable IDE0290 // Use primary constructor

using System.Buffers;
using System.Runtime.CompilerServices;

namespace CoreClassLibrary.Objects.Pooling;

/// <summary>
/// Value type ListBuilder
/// </summary>
/// <see cref="https://github.com/dotnet/coreclr/blob/dbaf2957387c5290a680c8918779683194137b1d/src/System.Private.CoreLib/shared/System/Collections/Generic/ValueListBuilder.cs"/>
/// <see cref="https://habr.com/en/company/clrium/blog/443976/"/>
public ref struct ValueListBuilder<T>
    {
    private Span<T> span;
    private T[]? arrayFromPool = null;

    public ValueListBuilder(Span<T> initialSpan)
        {
        span = initialSpan;
        }

    public int Length { get; set; } = 0;

    public ref T this[int index] 
        => ref span[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(T item)
        {
        int pos = this.Length;
        if (pos >= span.Length)
            {
            Grow();
            }

        span[pos] = item;
        this.Length = pos + 1;
        }

    public readonly ReadOnlySpan<T> AsSpan() 
        => span[..this.Length];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
        {
        if (arrayFromPool != null)
            {
            ArrayPool<T>.Shared.Return(arrayFromPool);
            arrayFromPool = null;
            }
        }

    private void Grow()
        {
        T[] array = ArrayPool<T>.Shared.Rent((span.Length+1) * 2);

        span.TryCopyTo(array);

        T[]? toReturn = arrayFromPool;
        span = arrayFromPool = array;
        if (toReturn != null)
            {
            ArrayPool<T>.Shared.Return(toReturn);
            }
        }
    }
