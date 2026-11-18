namespace CoreClassLibrary.Collections;

/// <summary>
/// Cola de longitud fija, que no admite valores duplicados.
/// </summary>
public class UniqueBoundedQueue<T>(int capacity) : BoundedQueue<T>(capacity), IQueue<T>
    {
    public override void Enqueue(T item)
        {
        int index = this.IndexOf(item);
        if(index == -1)
            {
            base.Enqueue(item);
            }
        else if(index != this.Size - 1)
            {
            //si no estaba en el ultimo lugar, la pongo ahi
            T[] destinationArray = new T[this.InnerArray.Length];

            int innerIndex = this.MapInnerIndex(index);

            if (this.Head < this.Tail)
                {
                Array.Copy(this.InnerArray, this.Head,      destinationArray, 0,        index);
                Array.Copy(this.InnerArray, innerIndex + 1, destinationArray, index,    this.Size - index - 1);
                }
            else if(this.Head <= innerIndex)
                {
                int lengthSubString = this.InnerArray.Length - innerIndex - 1;

                Array.Copy(this.InnerArray, this.Head,      destinationArray, 0,                        index);
                Array.Copy(this.InnerArray, innerIndex + 1, destinationArray, index,                    lengthSubString);
                Array.Copy(this.InnerArray, 0,              destinationArray, index + lengthSubString,   this.Tail);
                }
            else
                {
                int lengthSubString = this.InnerArray.Length - this.Head;

                Array.Copy(this.InnerArray, this.Head,      destinationArray, 0,                                lengthSubString);
                Array.Copy(this.InnerArray, 0,              destinationArray, lengthSubString,                 innerIndex);
                Array.Copy(this.InnerArray, innerIndex + 1, destinationArray, lengthSubString + innerIndex,    this.Tail - innerIndex - 1);
                }
                
            destinationArray[this.Size - 1] = item;

            this.InnerArray = destinationArray;
            this.Head = 0;

            this.Tail = this.Size == this.InnerArray.Length ? 0 : this.Size;
            }
        }
    }
