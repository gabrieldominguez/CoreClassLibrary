using CoreClassLibrary.Extensions;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CoreClassLibrary.Collections;

/// <summary>
/// Clase base para la implementacion de un fila y cola de longitud fija.
/// </summary>
/// <remarks>Implementado mediante un arreglo 'circular'. (Ver uso de %)</remarks>
[DebuggerDisplay("Count = {Count}"), ComVisible(false)]
public abstract class BoundedQueueStackBase<T> : IReadOnlyList<T>, ICollection
    {
    protected T[] InnerArray;
    protected int Size;
    protected int Head;
    protected int Tail;

    protected int Version;

    protected BoundedQueueStackBase(int capacity)
        {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        this.InnerArray = new T[capacity];
        this.Size = this.Tail = this.Head = 0;
        this.Version = 0;
        }

    public void Clear()
        {
        /*
        if (this.Head < this.Tail)
            {
            Array.Clear(this.InnerArray, this.Head, this.Size);
            }
        else
            {
            Array.Clear(this.InnerArray, this.Head, this.InnerArray.Length - this.Head);
            Array.Clear(this.InnerArray, 0, this.Tail);
            }
        */
        this.InnerArray.Clear();

        this.Size = this.Tail = this.Head = 0;
        this.Version++;
        }

    /// <summary>
    /// Searches for the specified object and returns the index of the first occurrence.
    /// </summary>
    /// <param name="item">The object to locate.</param>
    /// <returns>The zero-based index of the first occurrence of value within the entire array, if found; otherwise, –1. </returns>
    public int IndexOf (T item)
        {
        for (int i = 0; i < this.Size; i++)
			    {
            if(EqualityComparer<T>.Default.Equals(this[i], item))
                {
                return i;
                }
			    }

        return -1;
        }

    public bool Contains(T item) => this.IndexOf(item) != -1;

    public void CopyTo(T[] array, int arrayIndex)
        {
        if (arrayIndex < 0 || arrayIndex > array.Length)
            {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

        int length = array.Length;
        if (length - arrayIndex < this.Size)
            {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

        int num2 = (length - arrayIndex < this.Size) ? (length - arrayIndex) : this.Size;
        if (num2 != 0)
            {
            int num3 = (this.InnerArray.Length - this.Head < num2) ? (this.InnerArray.Length - this.Head) : num2;
            Array.Copy(this.InnerArray, this.Head, array, arrayIndex, num3);
            num2 -= num3;
            if (num2 > 0)
                {
                Array.Copy(this.InnerArray, 0, array, arrayIndex + this.InnerArray.Length - this.Head, num2);
                }
            }
        }

    protected int MapInnerIndex(int i) => (this.Head + i) % this.InnerArray.Length;

    public Enumerator GetEnumerator() => new Enumerator(this);

    public void SetCapacity(int capacity)
        {
        if(capacity != this.InnerArray.Length)
            {
            T[] destinationArray = new T[capacity];
            if (this.Size > 0)
                {
                if(capacity < this.Size)
                    {
                    this.Size = capacity;
                    }

                if (this.Head < this.Tail)
                    {
                    Array.Copy(this.InnerArray, this.Head, destinationArray, 0, this.Size);
                    }
                else
                    {
                    int lengthSubString = this.InnerArray.Length - this.Head;
                    if (lengthSubString > capacity)
                        {
                        lengthSubString = capacity;
                        }

                    Array.Copy(this.InnerArray, this.Head,  destinationArray, 0,                lengthSubString);
                    Array.Copy(this.InnerArray, 0,          destinationArray, lengthSubString,  capacity - lengthSubString);
                    }
                }

            this.InnerArray = destinationArray;
            this.Head = 0;
            this.Tail = this.Size == this.InnerArray.Length ? 0 : this.Size;

            this.Version++;
            }
        }

    public int Capacity => this.InnerArray.Length;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

    void ICollection.CopyTo(Array array, int index)
        {
        if (array.Rank != 1)
            {
            throw new ArgumentException("The rank (number of dimensions) of the System.Array must be 1.", nameof(array));
            }
        if (array.GetLowerBound(0) != 0)
            {
            throw new ArgumentException("The lower bound of the System.Array must be 0.", nameof(array));
            }
        int length = array.Length;
        if (index < 0 || index > length)
            {
            throw new ArgumentOutOfRangeException(nameof(index));
            }
        if (length - index < this.Size)
            {
            throw new ArgumentOutOfRangeException(nameof(index));
            }
        int num2 = (length - index < this.Size) ? (length - index) : this.Size;
        if (num2 != 0)
            {
            int num3 = (this.InnerArray.Length - this.Head < num2) ? (this.InnerArray.Length - this.Head) : num2;
            Array.Copy(this.InnerArray, this.Head, array, index, num3);
            num2 -= num3;
            if (num2 > 0)
                {
                Array.Copy(this.InnerArray, 0, array, index + this.InnerArray.Length - this.Head, num2);
                }
            }
        }

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public T[] ToArray()
        {
        T[] destinationArray = new T[this.Size];
        if (this.Size != 0)
            {
            if (this.Head < this.Tail)
                {
                Array.Copy(this.InnerArray, this.Head, destinationArray, 0, this.Size);
                return destinationArray;
                }
            Array.Copy(this.InnerArray, this.Head, destinationArray, 0, this.InnerArray.Length - this.Head);
            Array.Copy(this.InnerArray, 0, destinationArray, this.InnerArray.Length - this.Head, this.Tail);
            }
        return destinationArray;
        }

    public T this[int i] => this.InnerArray[this.MapInnerIndex(i)];

    public int Count => this.Size;

    protected bool IsSynchronized => false;

    bool ICollection.IsSynchronized => false;

    public object SyncRoot { get; } = new object();

    [StructLayout(LayoutKind.Sequential)]
    public record struct Enumerator : IEnumerator<T>
        {
        private readonly BoundedQueueStackBase<T> queue;
        private int index;
        private readonly int version;
        private T? currentElement;

        internal Enumerator(BoundedQueueStackBase<T> q)
            {
            this.queue = q;
            this.version = this.queue.Version;
            this.index = -1;
            this.currentElement = default;
            }

        public void Dispose()
            {
            this.index = -2;
            this.currentElement = default;
            }

        public bool MoveNext()
            {
            if (this.version != this.queue.Version)
                {
                throw new InvalidOperationException();
                }

            if (this.index == -2)
                {
                return false;
                }

            this.index++;

            if (this.index == this.queue.Size)
                {
                this.index = -2;
                this.currentElement = default;
                return false;
                }

            this.currentElement = this.queue[this.index];

            return true;
            }

        public readonly T Current 
            => this.index < 0 ? throw new InvalidOperationException() : this.currentElement!;

        readonly object IEnumerator.Current 
            => this.index < 0 ? throw new InvalidOperationException() : this.currentElement!;

        void IEnumerator.Reset()
            {
            if (this.version != this.queue.Version)
                {
                throw new InvalidOperationException();
                }
            this.index = -1;
            this.currentElement = default;
            }
        }
    }
