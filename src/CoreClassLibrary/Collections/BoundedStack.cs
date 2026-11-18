using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CoreClassLibrary.Collections;

/// <summary>
/// Pila de longitud fija. 
/// Cuando está lleno y se agrega un nuevo elemento se quita el más antiguo.
/// </summary>
/// <remarks>Implementado mediante un arreglo 'circular'. (Ver uso de %)</remarks>
[DebuggerDisplay("Count = {Count}"), ComVisible(false)]
public class BoundedStack<T>(int capacity) : BoundedQueueStackBase<T?>(capacity), IStack<T?>
    {
    public T? Peek()
        {
        if (this.Size == 0)
            {
            throw new InvalidOperationException();
            }

        int peekIndex = this.Tail - 1;
        if(peekIndex < 0)
            {
            peekIndex = this.InnerArray.Length;
            }

        return this.InnerArray[peekIndex];
        }

    public virtual T? Pop()
        {
        if (this.Size == 0)
            {
            throw new InvalidOperationException();
            }

        if(--this.Tail < 0)
            {
            this.Tail = this.InnerArray.Length - 1;
            }

        T? local = this.InnerArray[this.Tail];
        this.InnerArray[this.Tail] = default;

        this.Size--;
        this.Version++;

        return local;
        }

    public virtual void Push(T? item)
        {
        if(this.Size == this.InnerArray.Length)
            {
            //Quito el mas antiguo
            this.InnerArray[this.Head] = default;
            this.Head = (this.Head + 1) % this.InnerArray.Length;
            this.Size--;
            }

        this.InnerArray[this.Tail] = item;
        this.Tail = (this.Tail + 1) % this.InnerArray.Length;
        this.Size++;
        this.Version++;
        }
    }
