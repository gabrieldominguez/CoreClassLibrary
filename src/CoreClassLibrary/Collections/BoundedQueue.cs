using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CoreClassLibrary.Collections;

/// <summary>
/// Cola de longitud fija.
/// Cuando está llena y se agrega un nuevo elemento se quita el más antiguo.
/// </summary>
/// <remarks>Implementado mediante un arreglo 'circular'. (Ver uso de %)</remarks>
[DebuggerDisplay("Count = {Count}"), ComVisible(false)]
public class BoundedQueue<T>(int capacity) : BoundedQueueStackBase<T>(capacity), IQueue<T>
    {
    public T Peek()
        => this.Size == 0 ?
            throw new InvalidOperationException() :
            this.InnerArray[this.Head];

    public virtual T Dequeue()
        {
        if (this.Size == 0)
            {
            throw new InvalidOperationException();
            }

        T local = this.InnerArray[this.Head];
        this.Head = (this.Head + 1) % this.InnerArray.Length;
        this.Size--;
        this.Version++;

        return local;
        }

    public virtual void Enqueue(T item)
        {
        if(this.Size == this.InnerArray.Length)
            {
            this.Dequeue();
            }

        this.InnerArray[this.Tail] = item;
        this.Tail = (this.Tail + 1) % this.InnerArray.Length;
        this.Size++;
        this.Version++;
        }
    }
